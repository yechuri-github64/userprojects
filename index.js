<<<<<<< HEAD
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
=======
const AccountsHandler = require('./handlers/AccountsHandler');

exports.handler = async (event) => {
  console.log('Lambda invoked with event:', JSON.stringify(event));
  try {
    const response = await AccountsHandler.handle(event);
    console.log('Response:', JSON.stringify(response));
    return response;
  } catch (err) {
    console.log('Unhandled error:', err);
    return {
      statusCode: 500,
      body: JSON.stringify({ message: 'Internal server error' })
    };
  }
};
>>>>>>> bd324d8 (Automated commit on branch accounts-management-lambda from AI2DEV)
