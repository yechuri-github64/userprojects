const OrdersHandler = require('./handlers/OrdersHandler');

exports.handler = async (event, context) => {
  return OrdersHandler.handle(event, context);
};