require('dotenv').config();
const OrderHandler = require('./handlers/OrderHandler');
const response = require('./utils/response');

module.exports.handler = async (event) => {
  console.log('Lambda invoked with event:', JSON.stringify(event));
  try {
    const result = await OrderHandler.getOrder(event);
    return result;
  } catch (err) {
    console.log('Unhandled error in lambda handler:', err);
    return response.failure(500, { message: 'Internal server error' });
  }
};
