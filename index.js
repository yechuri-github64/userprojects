exports.handler = async (event, context) => {
  const handler = require('./handlers/OrdersHandler');
  try {
    return await handler.handle(event);
  } catch (err) {
    console.log('Unhandled error', err);
    return {
      statusCode: 500,
      body: JSON.stringify({ message: 'Internal server error' })
    };
  }
};