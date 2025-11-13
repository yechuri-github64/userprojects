require('dotenv').config();
const OrderHandler = require('./handlers/OrderHandler');

exports.handler = async (event, context) => {
  try {
    console.log('Lambda invoked', { event });
    const response = await OrderHandler.handle(event);
    return response;
  } catch (err) {
    console.log('Unhandled error in lambda', err);
    return {
      statusCode: 500,
      body: JSON.stringify({ message: 'Internal server error' })
    };
  }
};
