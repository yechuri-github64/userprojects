const SalesforceService = require('../services/SalesforceService');

async function parseOrderIdFromEvent(event) {
  // support pathParameters, queryStringParameters and JSON body
  if (event.pathParameters && event.pathParameters.orderId) return event.pathParameters.orderId;
  if (event.queryStringParameters && event.queryStringParameters.orderId) return event.queryStringParameters.orderId;
  if (event.body) {
    try {
      const body = JSON.parse(event.body);
      if (body && body.orderId) return body.orderId;
    } catch (err) {
      // not JSON body - ignore
    }
  }
  return null;
}

async function handle(event, context) {
  console.log('Incoming event:', JSON.stringify(event));
  try {
    const orderId = await parseOrderIdFromEvent(event);
    let data;
    if (orderId) {
      console.log('Fetching order by id:', orderId);
      const record = await SalesforceService.getOrderById(orderId);
      if (!record) {
        return {
          statusCode: 404,
          body: JSON.stringify({ success: false, message: 'Order not found', orderId })
        };
      }
      data = record;
    } else {
      console.log('Fetching all orders');
      const records = await SalesforceService.getAllOrders();
      data = records;
    }

    const response = {
      statusCode: 200,
      body: JSON.stringify({ success: true, data })
    };
    console.log('Response:', response);
    return response;
  } catch (error) {
    console.log('Error handling request:', error && error.message ? error.message : error);
    const statusCode = (error && error.status) || 500;
    return {
      statusCode,
      body: JSON.stringify({ success: false, message: 'Internal server error' })
    };
  }
}

module.exports = { handle };