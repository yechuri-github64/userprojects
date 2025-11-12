const OrderHandler = require('./handlers/OrderHandler');

exports.handler = async (event) => {
  console.log('Lambda invoked');
  try {
    const payload = typeof event.body === 'string' ? JSON.parse(event.body) : event;
    const result = await OrderHandler.handle(payload);
    return { statusCode: 200, body: JSON.stringify(result) };
  } catch (err) {
    console.log('Handler error', err);
    return { statusCode: err.statusCode || 500, body: JSON.stringify({ message: err.message || 'Internal server error' }) };
  }
};
