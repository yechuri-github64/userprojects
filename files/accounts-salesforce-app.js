'use strict';
const jsforce = require("jsforce");

exports.handler = async (event) => {
  try {
    const conn = new jsforce.Connection({
      loginUrl: process.env.SF_LOGIN_URL || 'https://login.salesforce.com'
    });

    await conn.login(
      process.env.SF_USERNAME,
      (process.env.SF_PASSWORD || '') + (process.env.SF_TOKEN || '')
    );

    const method = (event.httpMethod || (event.requestContext && event.requestContext.http && event.requestContext.http.method) || event.method) || 'POST';
    const query = event.queryStringParameters || (event.queryParameters || {});
    let body = {};
    if (event.body) {
      try {
        body = typeof event.body === 'string' ? JSON.parse(event.body) : event.body;
      } catch (e) {
        return formatError(400, 'Invalid JSON body', e.message);
      }
    }

    function success(data) {
      return {
        statusCode: 200,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ data })
      };
    }

    function formatError(status, message, details) {
      const err = { error: { message, details, status } };
      console.error('ERROR', message, details);
      return {
        statusCode: status || 500,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(err)
      };
    }

    switch ((method || '').toUpperCase()) {
      case 'POST': {
        // Create multiple accounts in one call. Accepts either an array in the body or body.records
        const records = Array.isArray(body) ? body : (Array.isArray(body.records) ? body.records : (body.records ? [body.records] : []));
        if (!records || records.length === 0) return formatError(400, 'No records provided', 'Provide an array of account objects in body or body.records');
        const createResult = await conn.sobject('Account').create(records);
        return success({ created: createResult });
      }

      case 'GET': {
        // Retrieve a single account by id (query.id or body.id) or perform a query (query.soql) or list (default limited query)
        const id = (query && query.id) || (body && (body.id || body.Id));
        if (id) {
          const acc = await conn.sobject('Account').retrieve(id);
          return success(acc);
        } else {
          const soql = (query && query.soql) || 'SELECT Id, Name, Phone, Website FROM Account LIMIT ' + ((query && query.limit) || 200);
          const res = await conn.query(soql);
          return success({ totalSize: res.totalSize, records: res.records });
        }
      }

      case 'PATCH': {
        // Update exactly one account at a time. id must be provided in body.Id or body.id or query.id
        const updateId = (body && (body.Id || body.id)) || (query && query.id);
        if (!updateId) return formatError(400, 'Missing id for update', 'Provide id in body.Id or body.id or query string');
        const updateFields = Object.assign({}, body);
        delete updateFields.Id; delete updateFields.id;
        if (Object.keys(updateFields).length === 0) return formatError(400, 'No fields to update', 'Provide fields to update in body');
        const updateRes = await conn.sobject('Account').update(Object.assign({ Id: updateId }, updateFields));
        return success({ updated: updateRes });
      }

      case 'DELETE': {
        // Delete a single account. id must be provided in query.id or body.id
        const deleteId = (query && query.id) || (body && (body.id || body.Id));
        if (!deleteId) return formatError(400, 'Missing id for delete', 'Provide id in query.id or body.id');
        const delRes = await conn.sobject('Account').destroy(deleteId);
        return success({ deleted: delRes });
      }

      default:
        return formatError(405, 'Method not allowed', 'Allowed: GET, POST, PATCH, DELETE');
    }
  } catch (err) {
    console.error('Unhandled error', err);
    const errObj = { error: { message: err.message || 'Unknown error', stack: err.stack, name: err.name } };
    return {
      statusCode: 500,
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(errObj)
    };
  }
};