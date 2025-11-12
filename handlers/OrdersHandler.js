module.exports.handle = async (event) => {
  console.log('Incoming event:', JSON.stringify(event));
  const SalesforceService = require('../services/SalesforceService');
  try {
    const method = event.httpMethod;
    const orderId = event.pathParameters && event.pathParameters.orderId;

    if (method === 'GET') {
      if (!orderId) {
        return { statusCode: 400, body: JSON.stringify({ message: 'orderId is required' }) };
      }
      const order = await SalesforceService.getOrder(orderId);
      return { statusCode: 200, body: JSON.stringify(order) };
    }

    if (method === 'PUT') {
      if (!orderId) {
        return { statusCode: 400, body: JSON.stringify({ message: 'orderId is required' }) };
      }
      if (!event.body) {
        return { statusCode: 400, body: JSON.stringify({ message: 'request body is required' }) };
      }
      const payload = JSON.parse(event.body);
      const result = await SalesforceService.updateOrder(orderId, payload);
      return { statusCode: 200, body: JSON.stringify(result) };
    }

    return { statusCode: 405, body: JSON.stringify({ message: 'Method not allowed' }) };
  } catch (err) {
    console.log('Handler error:', err);
    const statusCode = err.statusCode || 500;
    return { statusCode, body: JSON.stringify({ message: err.message || 'Internal error' }) };
  }
};