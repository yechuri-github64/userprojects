const SalesforceService = require('../services/SalesforceService');

module.exports.handle = async (input) => {
  console.log('OrderHandler input:', JSON.stringify(input));
  try {
    const payload = input && input.order ? input.order : input;
    if (!payload) throw Object.assign(new Error('Missing order payload'), { statusCode: 400 });

    const orderData = typeof payload === 'string' ? JSON.parse(payload) : payload;

    const created = await SalesforceService.createOrder(orderData);
    console.log('Salesforce createOrder response:', JSON.stringify(created));
    return { success: true, salesforce: created };
  } catch (err) {
    console.log('OrderHandler error', err);
    throw err;
  }
};
