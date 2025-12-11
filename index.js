const { handle } = require('./handlers/UserHandler');
exports.handler = async (event, context) => {
  return await handle(event, context);
};