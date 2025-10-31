const SalesforceService = require('../services/SalesforceService');
const { removeEmptyFields } = require('../utils/cleaner');

const jsonHeaders = { 'Content-Type': 'application/json' };

async function handleGet(accountId) {
  if (!accountId) {
    return { statusCode: 400, headers: jsonHeaders, body: JSON.stringify({ message: 'Missing accountId in path parameters' }) };
  }
  const account = await SalesforceService.getAccount(accountId);
  const cleaned = removeEmptyFields(account);
  return { statusCode: 200, headers: jsonHeaders, body: JSON.stringify(cleaned) };
}

async function handlePost(body) {
  if (!body) {
    return { statusCode: 400, headers: jsonHeaders, body: JSON.stringify({ message: 'Missing request body' }) };
  }
  let payload;
  try {
    payload = typeof body === 'string' ? JSON.parse(body) : body;
  } catch (err) {
    return { statusCode: 400, headers: jsonHeaders, body: JSON.stringify({ message: 'Invalid JSON body' }) };
  }
  const cleaned = removeEmptyFields(payload);
  const result = await SalesforceService.createAccount(cleaned);
  return { statusCode: 201, headers: jsonHeaders, body: JSON.stringify(result) };
}

async function handlePut(accountId, body) {
  if (!accountId) {
    return { statusCode: 400, headers: jsonHeaders, body: JSON.stringify({ message: 'Missing accountId in path parameters' }) };
  }
  if (!body) {
    return { statusCode: 400, headers: jsonHeaders, body: JSON.stringify({ message: 'Missing request body' }) };
  }
  let payload;
  try {
    payload = typeof body === 'string' ? JSON.parse(body) : body;
  } catch (err) {
    return { statusCode: 400, headers: jsonHeaders, body: JSON.stringify({ message: 'Invalid JSON body' }) };
  }
  const cleaned = removeEmptyFields(payload);
  await SalesforceService.updateAccount(accountId, cleaned);
  return { statusCode: 204, headers: jsonHeaders, body: '' };
}

async function handleDelete(accountId) {
  if (!accountId) {
    return { statusCode: 400, headers: jsonHeaders, body: JSON.stringify({ message: 'Missing accountId in path parameters' }) };
  }
  await SalesforceService.deleteAccount(accountId);
  return { statusCode: 204, headers: jsonHeaders, body: '' };
}

exports.handler = async (event) => {
  console.log('Event received:', JSON.stringify(event));
  try {
    const method = (event.httpMethod || (event.requestContext && event.requestContext.http && event.requestContext.http.method) || 'GET').toUpperCase();
    const pathParams = event.pathParameters || {};
    const accountId = pathParams.accountid || pathParams.accountId || pathParams.id;

    if (method === 'GET') {
      return await handleGet(accountId);
    }

    if (method === 'POST') {
      return await handlePost(event.body);
    }

    if (method === 'PUT' || method === 'PATCH') {
      return await handlePut(accountId, event.body);
    }

    if (method === 'DELETE') {
      return await handleDelete(accountId);
    }

    return { statusCode: 405, headers: jsonHeaders, body: JSON.stringify({ message: 'Method Not Allowed' }) };
  } catch (err) {
    console.log('Handler error:', err && err.message ? err.message : err);
    const statusCode = err && err.statusCode ? err.statusCode : 500;
    const message = err && err.message ? err.message : 'Internal Server Error';
    return { statusCode, headers: jsonHeaders, body: JSON.stringify({ message }) };
  }
};
