'use strict';

const jsforce = require('jsforce');

// Exported Lambda handler name must be 'accounts-salesforce-app'
exports['accounts-salesforce-app'] = async (event, context) => {
  const respond = (statusCode, payload) => ({
    statusCode,
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload)
  });

  try {
    // Validate and parse input
    const method = (event.httpMethod || (event.requestContext && event.requestContext.http && event.requestContext.http.method) || '').toUpperCase();
    let body = {};
    if (event.body) {
      try {
        body = typeof event.body === 'string' ? JSON.parse(event.body) : event.body;
      } catch (parseErr) {
        console.error('Invalid JSON body', parseErr);
        return respond(400, { error: { message: 'Invalid JSON in request body', code: 'InvalidJson', details: parseErr.message } });
      }
    }

    // Salesforce connection configuration comes from environment variables
    const SF_USERNAME = process.env.SF_USERNAME;
    const SF_PASSWORD = process.env.SF_PASSWORD;
    const SF_TOKEN = process.env.SF_TOKEN || '';
    const SF_LOGIN_URL = process.env.SF_LOGIN_URL || 'https://login.salesforce.com';

    if (!SF_USERNAME || !SF_PASSWORD) {
      const errMsg = 'Missing Salesforce credentials. Set SF_USERNAME and SF_PASSWORD environment variables.';
      console.error(errMsg);
      return respond(500, { error: { message: errMsg, code: 'ConfigError' } });
    }

    // Connect to Salesforce
    const conn = new jsforce.Connection({ loginUrl: SF_LOGIN_URL });
    try {
      await conn.login(SF_USERNAME, SF_PASSWORD + SF_TOKEN);
    } catch (loginErr) {
      console.error('Salesforce login error', loginErr);
      return respond(500, { error: { message: 'Failed to login to Salesforce', code: 'SalesforceLoginError', details: loginErr && loginErr.message ? loginErr.message : loginErr } });
    }

    // ROUTING
    if (method === 'POST') {
      // Create multiple accounts in a single JSON: { "accounts": [ { Name: "A" , ... }, ... ] }
      if (!body.accounts || !Array.isArray(body.accounts)) {
        return respond(400, { error: { message: 'Invalid input. Expected JSON with an "accounts" array.', code: 'InvalidInput' } });
      }

      try {
        // allOrNone: false to allow partial successes
        const results = await conn.sobject('Account').create(body.accounts, { allOrNone: false });
        // Normalize result to arrays of successes and errors
        const successes = [];
        const errors = [];
        if (Array.isArray(results)) {
          results.forEach((r, idx) => {
            if (r && r.success) {
              successes.push({ index: idx, id: r.id });
            } else {
              errors.push({ index: idx, error: r });
            }
          });
        } else if (results && results.success) {
          successes.push({ index: 0, id: results.id });
        }

        return respond(200, { created: successes, errors });
      } catch (createErr) {
        console.error('Create error', createErr);
        return respond(500, { error: { message: 'Failed to create accounts', code: 'CreateError', details: createErr && createErr.message ? createErr.message : createErr } });
      }
    }

    if (method === 'GET') {
      // Retrieve one or many. Query params expected in event.queryStringParameters
      const qsp = event.queryStringParameters || {};
      try {
        if (qsp.id) {
          // Get single account by Id
          const account = await conn.sobject('Account').retrieve(qsp.id);
          return respond(200, { account });
        }

        if (qsp.soql) {
          // Allow users to pass a SOQL
          const result = await conn.query(qsp.soql);
          return respond(200, { totalSize: result.totalSize, done: result.done, records: result.records });
        }

        // Default: return up to 200 accounts with common fields
        const defaultQuery = 'SELECT Id, Name, Type, Phone, Industry, BillingStreet, BillingCity, BillingState, BillingPostalCode, BillingCountry FROM Account LIMIT 200';
        const result = await conn.query(defaultQuery);
        return respond(200, { totalSize: result.totalSize, done: result.done, records: result.records });
      } catch (getErr) {
        console.error('Get error', getErr);
        return respond(500, { error: { message: 'Failed to retrieve accounts', code: 'RetrieveError', details: getErr && getErr.message ? getErr.message : getErr } });
      }
    }

    if (method === 'PATCH' || method === 'PUT') {
      // Update one account at a time. Input expected: { "id": "001...", "fields": { "Name": "New" , ... } }
      if (!body.id || !body.fields || typeof body.fields !== 'object') {
        return respond(400, { error: { message: 'Invalid input. Expected { "id": "<Id>", "fields": { ... } }', code: 'InvalidInput' } });
      }

      const updateObj = Object.assign({ Id: body.id }, body.fields);
      try {
        const res = await conn.sobject('Account').update(updateObj);
        if (res && res.success) {
          return respond(200, { updated: { id: res.id } });
        }
        // If jsforce returns a non-success object
        return respond(400, { error: { message: 'Update failed', code: 'UpdateFailed', details: res } });
      } catch (updateErr) {
        console.error('Update error', updateErr);
        return respond(500, { error: { message: 'Failed to update account', code: 'UpdateError', details: updateErr && updateErr.message ? updateErr.message : updateErr } });
      }
    }

    if (method === 'DELETE') {
      // Delete by id(s). Accept query param id (single or comma separated) or body.ids array or body.id single.
      const qsp = event.queryStringParameters || {};
      let ids = [];
      if (qsp.id) {
        ids = qsp.id.split(',').map(s => s.trim()).filter(Boolean);
      } else if (body.ids && Array.isArray(body.ids)) {
        ids = body.ids;
      } else if (body.id) {
        ids = [body.id];
      } else {
        return respond(400, { error: { message: 'Invalid input. Provide id in query string or body (id or ids).', code: 'InvalidInput' } });
      }

      try {
        if (ids.length === 1) {
          const res = await conn.sobject('Account').destroy(ids[0]);
          if (res && res.success) {
            return respond(200, { deleted: [{ id: ids[0] }] });
          }
          return respond(400, { error: { message: 'Delete failed', code: 'DeleteFailed', details: res } });
        } else {
          // delete multiple
          const res = await conn.sobject('Account').del(ids);
          // jsforce returns array of results for bulk delete
          const deleted = [];
          const errors = [];
          if (Array.isArray(res)) {
            res.forEach((r, i) => {
              if (r && r.success) deleted.push({ index: i, id: ids[i] });
              else errors.push({ index: i, id: ids[i], error: r });
            });
          }
          return respond(200, { deleted, errors });
        }
      } catch (delErr) {
        console.error('Delete error', delErr);
        return respond(500, { error: { message: 'Failed to delete account(s)', code: 'DeleteError', details: delErr && delErr.message ? delErr.message : delErr } });
      }
    }

    // Unsupported method
    return respond(405, { error: { message: 'Method not allowed', code: 'MethodNotAllowed', method } });

  } catch (err) {
    // Catch-all error handling: log and return structured error
    console.error('Unhandled error', err);
    const structured = {
      error: {
        message: err && err.message ? err.message : 'Unknown error',
        code: err && err.name ? err.name : 'UnhandledError',
        details: err && err.stack ? err.stack : err
      }
    };
    return { statusCode: 500, headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(structured) };
  }
};
