const response = require('../utils/response');
const SalesforceService = require('../services/SalesforceService');

exports.getOrder = async (event) => {
  try {
    console.log('OrderHandler.getOrder event:', JSON.stringify(event));

    let orderId;

    if (event.pathParameters && event.pathParameters.orderId) {
      orderId = event.pathParameters.orderId;
    } else if (event.queryStringParameters && event.queryStringParameters.orderId) {
      orderId = event.queryStringParameters.orderId;
    } else if (event.body) {
      const body = typeof event.body === 'string' ? JSON.parse(event.body) : event.body;
      orderId = body.orderId || body.id || body.Id;
    }

    if (!orderId) {
      console.log('No orderId provided');
      return response.failure(400, { message: 'Missing orderId' });
    }

    const order = await SalesforceService.getOrderById(orderId);

    if (!order) {
      return response.failure(404, { message: 'Order not found' });
    }

    return response.success(200, order);
  } catch (err) {
    console.log('Error in OrderHandler.getOrder:', err);
    const status = err && err.statusCode ? err.statusCode : 500;
    const message = err && err.message ? err.message : 'Error retrieving order';
    return response.failure(status, { message });
  }
};
