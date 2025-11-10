const AccountsService = require('../services/AccountsService');

const jsonResponse = (statusCode, payload) => ({
  statusCode,
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify(payload)
});

const parseBody = (body) => {
  if (!body) return null;
  try {
    return JSON.parse(body);
  } catch (err) {
    throw new Error('Invalid JSON body');
  }
};

exports.handle = async (event) => {
  const method = (event.httpMethod || 'GET').toUpperCase();
  try {
    if (method === 'GET') {
      const id = event.pathParameters && event.pathParameters.id
        ? event.pathParameters.id
        : (event.queryStringParameters && event.queryStringParameters.id) || null;
      if (id) {
        const account = await AccountsService.getAccount(id);
        if (!account) return jsonResponse(404, { message: 'Account not found' });
        return jsonResponse(200, account);
      }
      const accounts = await AccountsService.getAllAccounts();
      return jsonResponse(200, accounts);
    }

    if (method === 'POST') {
      const body = parseBody(event.body);
      if (!Array.isArray(body)) return jsonResponse(400, { message: 'Body must be a JSON array of accounts' });
      // Validate minimal shape for each account (name, email, address optional but allow empty strings)
      const created = await AccountsService.createAccounts(body);
      return jsonResponse(201, { message: 'Accounts created', createdCount: created.insertedCount || created.affectedRows || 0 });
    }

    if (method === 'PUT') {
      const body = parseBody(event.body) || {};
      const id = (event.pathParameters && event.pathParameters.id) || body.id || (event.queryStringParameters && event.queryStringParameters.id);
      if (!id) return jsonResponse(400, { message: 'Missing id for update' });
      const updated = await AccountsService.updateAccount(id, body);
      if (updated.affectedRows === 0) return jsonResponse(404, { message: 'Account not found' });
      return jsonResponse(200, { message: 'Account updated' });
    }

    if (method === 'DELETE') {
      const id = (event.pathParameters && event.pathParameters.id) || (event.queryStringParameters && event.queryStringParameters.id);
      if (!id) return jsonResponse(400, { message: 'Missing id for delete' });
      const deleted = await AccountsService.deleteAccount(id);
      if (deleted.affectedRows === 0) return jsonResponse(404, { message: 'Account not found' });
      return jsonResponse(200, { message: 'Account deleted' });
    }

    return jsonResponse(405, { message: 'Method Not Allowed' });
  } catch (err) {
    console.log('Handler error:', err && err.message ? err.message : err);
    const status = err.message === 'Invalid JSON body' ? 400 : 500;
    return jsonResponse(status, { message: err.message || 'Internal error' });
  }
};