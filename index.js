require('dotenv').config();
const OrderHandler = require('./handlers/OrderHandler');

exports.handler = async (event, context) => {
  console.log('Lambda invoked', { event });
  try {
    const result = await OrderHandler.handle(event, context);
    return result;
  } catch (err) {
    console.error('Unhandled error', err);
    return {
      statusCode: 500,
      body: JSON.stringify({ error: 'Internal server error' })
    };
  }
};
