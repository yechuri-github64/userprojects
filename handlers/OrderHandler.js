const SalesforceService = require('../services/SalesforceService');
const { pruneEmpty } = require('../utils/utils');

// Lambda handler to retrieve an Order by Id
exports.handler = async (event) => {
  console.log('Received event:', JSON.stringify(event));

  try {
    let payload = {};
    if (event.body) {
      try {
        payload = JSON.parse(event.body);
      } catch (err) {
        // body might already be an object
        payload = event.body;
      }
    }

    const orderId = (event.pathParameters && event.pathParameters.orderId) || payload.orderId || event.orderId;

    if (!orderId) {
      console.log('Missing orderId');
      return {
        statusCode: 400,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ message: 'orderId is required' })
      };
    }

    console.log('Fetching order for Id:', orderId);
    const order = await SalesforceService.getOrder(orderId);

    if (!order) {
      return {
        statusCode: 404,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ message: 'Order not found' })
      };
    }

    // Ensure response has no empty fields
    const cleaned = pruneEmpty(order);

    return {
      statusCode: 200,
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(cleaned)
    };
  } catch (err) {
    console.log('Error in handler:', err && err.message ? err.message : err);
    return {
      statusCode: 500,
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ message: 'Internal server error' })
    };
  }
};
