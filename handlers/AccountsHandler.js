const SalesforceService = require('../services/SalesforceService');
const Response = require('../utils/response');

const parseBody = (body) => {
  if (!body) return {};
  try {
    return JSON.parse(body);
  } catch (err) {
    throw { statusCode: 400, message: 'Invalid JSON body' };
  }
};

exports.handle = async (event) => {
  const method = (event.httpMethod || '').toUpperCase();
  const id = event.pathParameters && event.pathParameters.id;
  try {
    if (method === 'POST' && event.path === '/accounts') {
      const data = parseBody(event.body);
      console.log('Create account payload', JSON.stringify(data));
      const result = await SalesforceService.createAccount(data);
      return Response.ok(result);
    }

    if (method === 'GET') {
      if (id) {
        const account = await SalesforceService.getAccount(id);
        return Response.ok(account);
      }
      // list accounts - basic limited list
      const q = event.queryStringParameters && event.queryStringParameters.q;
      const list = await SalesforceService.listAccounts(q);
      return Response.ok(list);
    }

    if (method === 'PUT' && id) {
      const data = parseBody(event.body);
      console.log('Update account', id, JSON.stringify(data));
      const result = await SalesforceService.updateAccount(id, data);
      return Response.ok(result);
    }

    if (method === 'DELETE' && id) {
      console.log('Delete account', id);
      const result = await SalesforceService.deleteAccount(id);
      return Response.ok(result);
    }

    return Response.notFound({ message: 'Route not found' });
  } catch (err) {
    console.log('Handler error', err && err.stack ? err.stack : err);
    const status = err && err.statusCode ? err.statusCode : 500;
    const body = err && err.message ? { message: err.message } : { message: 'Internal server error' };
    return { statusCode: status, body: JSON.stringify(body) };
  }
};
