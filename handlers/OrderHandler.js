require('dotenv').config();
const SalesforceService = require('../services/SalesforceService');
const { successResponse, errorResponse, parseJson } = require('../utils/response');

const OrderHandler = {
  handle: async (event) => {
    try {
      const body = parseJson(typeof event.body === 'string' ? event.body : JSON.stringify(event.body || {}));
      if (!body) return errorResponse(400, 'Invalid JSON body');

      const orderId = body.Id || body.orderId;
      if (!orderId) return errorResponse(400, 'Missing order Id (Id or orderId)');

      // Prepare payload: clone body and remove Id/orderId
      const payload = Object.assign({}, body);
      delete payload.Id;
      delete payload.orderId;

      console.log('OrderHandler: updating Order', { orderId, payload });

      const svc = new SalesforceService();
      const updated = await svc.updateOrder(orderId, payload);

      console.log('OrderHandler: update result', { updated });
      return successResponse(200, updated);
    } catch (err) {
      console.error('OrderHandler error', err);
      const status = err && err.statusCode ? err.statusCode : 500;
      const message = err && err.message ? err.message : 'Internal error';
      return errorResponse(status, message);
    }
  }
};

module.exports = OrderHandler;
