const SalesforceService = require('../services/SalesforceService');
const { successResponse, errorResponse } = require('../utils/response');

const parseEventBody = (event) => {
  if (!event) return {};
  if (event.body) {
    try {
      return JSON.parse(event.body);
    } catch (e) {
      throw new Error('Invalid JSON in request body');
    }
  }
  return event;
};

exports.handle = async (event) => {
  console.log('OrderHandler.handle start');
  try {
    const body = parseEventBody(event);

    // Determine orderId: prefer pathParameters, then body.orderId, then queryStringParameters
    let orderId = null;
    if (event && event.pathParameters && event.pathParameters.orderId) orderId = event.pathParameters.orderId;
    if (!orderId && body && body.orderId) orderId = body.orderId;
    if (!orderId && event && event.queryStringParameters && event.queryStringParameters.orderId) orderId = event.queryStringParameters.orderId;

    if (!orderId) {
      console.log('Missing orderId');
      return errorResponse(400, 'Missing orderId');
    }

    // Remove orderId from update payload if present
    const { orderId: _discard, ...orderPayload } = body || {};

    if (!orderPayload || Object.keys(orderPayload).length === 0) {
      console.log('Missing order update payload');
      return errorResponse(400, 'Missing order update payload');
    }

    const result = await SalesforceService.updateOrder(orderId, orderPayload);
    console.log('Salesforce update result', result);
    return successResponse(200, { message: 'Order updated', result });
  } catch (err) {
    console.log('Error in OrderHandler', err && err.message ? err.message : err);
    const status = err && err.statusCode ? err.statusCode : 500;
    const message = err && err.message ? err.message : 'Internal server error';
    return errorResponse(status, message);
  }
};
