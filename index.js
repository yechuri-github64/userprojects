require('dotenv').config();
const AccountsHandler = require('./handlers/AccountsHandler');
const response = require('./utils/response');
exports.handler = async (event) => {
  console.log('incoming event:', JSON.stringify(event));
  try {
    const method = event.httpMethod || (event.requestContext && event.requestContext.http && event.requestContext.http.method);
    if (method === 'GET') {
      return await AccountsHandler.get(event);
    }
    if (method === 'POST') {
      return await AccountsHandler.create(event);
    }
    if (method === 'PUT' || method === 'PATCH') {
      return await AccountsHandler.update(event);
    }
    if (method === 'DELETE') {
      return await AccountsHandler.remove(event);
    }
    return response.format(405, { message: 'Method Not Allowed' });
  } catch (err) {
    console.log('handler error', err);
    return response.format(500, { error: 'Internal Server Error' });
  }
};