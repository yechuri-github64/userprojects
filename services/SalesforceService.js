require('dotenv').config();
const jsforce = require('jsforce');

let conn = null;

async function ensureConnection() {
  if (conn && conn.accessToken) return conn;

  const loginUrl = process.env.SALESFORCE_LOGIN_URL || 'https://login.salesforce.com';
  const username = process.env.SALESFORCE_USERNAME;
  const password = process.env.SALESFORCE_PASSWORD;
  const securityToken = process.env.SALESFORCE_SECURITY_TOKEN || '';

  if (!username || !password) {
    throw new Error('Salesforce credentials not configured');
  }

  const maxRetries = parseInt(process.env.SALESFORCE_MAX_RETRIES || '3', 10);
  const retryDelay = parseInt(process.env.SALESFORCE_RETRY_DELAY || '3', 10) * 1000;
  const apiVersion = process.env.SALESFORCE_API_VERSION || '59.0';

  let lastError = null;

  for (let attempt = 1; attempt <= maxRetries; attempt++) {
    try {
      conn = new jsforce.Connection({ loginUrl, version: apiVersion });
      await conn.login(username, password + securityToken);
      if (process.env.SALESFORCE_ENABLE_LOGGING === 'true') {
        console.log('Salesforce login successful');
      }
      return conn;
    } catch (err) {
      lastError = err;
      console.log(`Salesforce login attempt ${attempt} failed:`, err && err.message ? err.message : err);
      if (attempt < maxRetries) {
        await new Promise((resolve) => setTimeout(resolve, retryDelay));
      }
    }
  }

  throw lastError || new Error('Unable to connect to Salesforce');
}

async function getOrderById(orderId) {
  const connection = await ensureConnection();
  try {
    console.log('Fetching order from Salesforce with Id:', orderId);
    const fields = [
      'Id',
      'AccountId',
      'OrderNumber',
      'EffectiveDate',
      'ActivatedDate',
      'Status',
      'TotalAmount',
      'CurrencyIsoCode',
      'Description',
      'CreatedDate',
      'LastModifiedDate',
      'OwnerId',
      'IsDeleted',
      'BillingAddress',
      'ShippingAddress'
    ];

    try {
      const order = await connection.sobject('Order').retrieve(orderId);
      if (!order || !order.Id) return null;
      const result = {};
      fields.forEach((f) => {
        if (Object.prototype.hasOwnProperty.call(order, f)) result[f] = order[f];
      });
      return result;
    } catch (err) {
      console.log('Salesforce retrieve error:', err);
      throw err;
    }
  } catch (err) {
    console.log('Error in getOrderById:', err);
    throw err;
  }
}

module.exports = {
  getOrderById
};
