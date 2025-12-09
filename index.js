const AccountsHandler = require('./handlers/AccountsHandler');

module.exports.handler = async (event, context) => {
  console.log('Lambda invoked', JSON.stringify({ path: event.path, method: event.httpMethod, pathParameters: event.pathParameters, queryStringParameters: event.queryStringParameters }));
  try {
    const response = await AccountsHandler.handle(event);
    return response;
  } catch (err) {
    console.log('Unhandled error in lambda handler', err && err.stack ? err.stack : err);
    return {
      statusCode: 500,
      body: JSON.stringify({ message: 'Internal server error' })
    };
  }
};
