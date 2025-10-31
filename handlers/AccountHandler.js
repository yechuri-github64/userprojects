const AccountService = require('../services/AccountService');

const jsonResponse = (statusCode, body) => ({
  statusCode,
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify(body)
});

const parseBody = (body) => {
  if (!body) return null;
  try {
    return JSON.parse(body);
  } catch (err) {
    throw new Error('Invalid JSON body');
  }
};

const handler = async (event) => {
  console.log('Incoming event:', JSON.stringify(event));
  try {
    const method = (event.httpMethod || '').toUpperCase();
    const id = event.pathParameters && event.pathParameters.id;

    if (method === 'POST') {
      const payload = parseBody(event.body);
      if (!payload) return jsonResponse(400, { message: 'Request body required' });
      const created = await AccountService.createAccount(payload);
      return jsonResponse(201, created);
    }

    if (method === 'GET') {
      if (!id) return jsonResponse(400, { message: 'Account id required in pathParameters' });
      const account = await AccountService.getAccount(id);
      if (!account) return jsonResponse(404, { message: 'Account not found' });
      return jsonResponse(200, account);
    }

    if (method === 'PUT' || method === 'PATCH') {
      if (!id) return jsonResponse(400, { message: 'Account id required in pathParameters' });
      const payload = parseBody(event.body);
      if (!payload) return jsonResponse(400, { message: 'Request body required' });
      const updated = await AccountService.updateAccount(id, payload);
      return jsonResponse(200, updated);
    }

    if (method === 'DELETE') {
      if (!id) return jsonResponse(400, { message: 'Account id required in pathParameters' });
      await AccountService.deleteAccount(id);
      return jsonResponse(204, {});
    }

    return jsonResponse(405, { message: `Method ${method} not allowed` });
  } catch (err) {
    console.log('Handler error:', err && err.message ? err.message : err);
    const status = err && err.statusCode ? err.statusCode : 500;
    const message = err && err.message ? err.message : 'Internal server error';
    return jsonResponse(status, { message });
  }
};

module.exports = { handler };
