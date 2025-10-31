const SalesforceService = require('../services/SalesforceService');
const { buildResponse } = require('../services/utils');

const service = new SalesforceService();

async function parseBody(body) {
  if (!body) return null;
  if (typeof body === 'string') return JSON.parse(body);
  return body;
}

exports.handler = async (event) => {
  console.log('Received event:', JSON.stringify({
    httpMethod: event.httpMethod,
    pathParameters: event.pathParameters,
    queryStringParameters: event.queryStringParameters
  }));

  try {
    const method = (event.httpMethod || 'GET').toUpperCase();

    // 👇 handles both /{id} and ?id=
    const id =
      (event.pathParameters && event.pathParameters.id) ||
      (event.queryStringParameters && event.queryStringParameters.id);

    const body = await parseBody(event.body);

    if (method === 'POST') {
      if (!body || typeof body !== 'object') {
        return buildResponse(400, { message: 'Invalid account object in request body' });
      }
      const created = await service.createAccount(body);
      return buildResponse(201, created);
    }

    if (method === 'GET') {
      if (!id) return buildResponse(400, { message: 'Missing account id in path or query' });
      const account = await service.getAccount(id);
      return buildResponse(200, account);
    }

    if (method === 'PUT' || method === 'PATCH') {
      if (!id) return buildResponse(400, { message: 'Missing account id in path or query' });
      if (!body || typeof body !== 'object') {
        return buildResponse(400, { message: 'Invalid account object in request body' });
      }
      await service.updateAccount(id, body);
      const updated = await service.getAccount(id);
      return buildResponse(200, updated);
    }

    if (method === 'DELETE') {
      if (!id) return buildResponse(400, { message: 'Missing account id in path or query' });
      await service.deleteAccount(id);
      return buildResponse(204, null);
    }

    return buildResponse(405, { message: 'Method Not Allowed' });
  } catch (err) {
    console.error('Handler error:', err && err.message ? err.message : err);
    const status = err && err.statusCode ? err.statusCode : 500;
    const message = err && err.message ? err.message : 'Internal Server Error';
    return buildResponse(status, { message });
  }
};
