const handler = require('./handlers/AccountsHandler');

exports.handler = async (event) => {
  try {
    console.log('Lambda received event:', JSON.stringify(event));
    const response = await handler.handle(event);
    return response;
  } catch (err) {
    console.log('Unhandled error in handler:', err);
    return {
      statusCode: 500,
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ message: 'Internal server error' })
    };
  }
};