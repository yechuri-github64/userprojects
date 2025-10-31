const SalesforceService = require('../services/SalesforceService');

function buildResponse(statusCode, body) {
  return {
    statusCode,
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(body)
  };
}

function parseJsonSafe(body) {
  if (!body) return null;
  try {
    return JSON.parse(body);
  } catch (err) {
    console.log('Failed to parse JSON body', err);
    throw new Error('Invalid JSON body');
  }
}

module.exports.handler = async (event, context) => {
  console.log('Incoming event', JSON.stringify({ path: event.path, httpMethod: event.httpMethod, pathParameters: event.pathParameters, queryStringParameters: event.queryStringParameters }));

  try {
    const method = (event.httpMethod || 'GET').toUpperCase();

    if (method === 'GET') {
      let accountId = null;

      if (event.pathParameters) {
        accountId = event.pathParameters.accountid || event.pathParameters.accountId || event.pathParameters.id || accountId;
      }

      if (!accountId && event.queryStringParameters) {
        accountId = event.queryStringParameters.accountid || event.queryStringParameters.accountId || event.queryStringParameters.id || accountId;
      }

      if (!accountId && event.path) {
        const parts = event.path.split('/').filter(Boolean);
        // support URL patterns like /account/{id}
        if (parts.length >= 2 && parts[parts.length - 2].toLowerCase() === 'account') {
          accountId = parts[parts.length - 1];
        }
      }

      if (!accountId) {
        return buildResponse(400, { message: 'Missing accountId in pathParameters or queryStringParameters' });
      }

      const account = await SalesforceService.getAccountById(accountId);
      return buildResponse(200, account);
    }

    // Minimal CRUD scaffolding for other methods
    if (method === 'POST') {
      // Create - not implemented
      const body = parseJsonSafe(event.body);
      console.log('POST body', body);
      return buildResponse(501, { message: 'Create operation not implemented in this function' });
    }

    if (method === 'PUT' || method === 'PATCH') {
      // Update - not implemented
      const body = parseJsonSafe(event.body);
      console.log(`${method} body`, body);
      return buildResponse(501, { message: 'Update operation not implemented in this function' });
    }

    if (method === 'DELETE') {
      // Delete - not implemented
      return buildResponse(501, { message: 'Delete operation not implemented in this function' });
    }

    return buildResponse(405, { message: 'Method Not Allowed' });
  } catch (err) {
    console.log('Handler error', err && err.message ? err.message : err);
    const message = err && err.message ? err.message : 'Internal Server Error';
    return buildResponse(500, { message });
  }
};
