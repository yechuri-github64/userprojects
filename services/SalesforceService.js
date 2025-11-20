require('dotenv').config();
const jsforce = require('jsforce');
const { pruneEmpty } = require('../utils/utils');

const MAX_RETRIES = parseInt(process.env.SALESFORCE_MAX_RETRIES || '3', 10);
const RETRY_DELAY = parseInt(process.env.SALESFORCE_RETRY_DELAY || '3', 10) * 1000;

async function connectWithRetry() {
  const conn = new jsforce.Connection({
    loginUrl: process.env.SALESFORCE_LOGIN_URL || undefined,
    version: process.env.SALESFORCE_API_VERSION || undefined
  });

  const username = process.env.SALESFORCE_USERNAME;
  const password = process.env.SALESFORCE_PASSWORD;
  const token = process.env.SALESFORCE_SECURITY_TOKEN || '';

  if (!username || !password) {
    throw new Error('Salesforce credentials are not set');
  }

  let attempt = 0;
  while (true) {
    attempt++;
    try {
      await conn.login(username, password + token);
      if (process.env.SALESFORCE_INSTANCE_URL) {
        conn.instanceUrl = process.env.SALESFORCE_INSTANCE_URL;
      }
      if (process.env.SALESFORCE_ENABLE_LOGGING === 'true') {
        console.log('Salesforce connected, instanceUrl:', conn.instanceUrl);
      }
      return conn;
    } catch (err) {
      console.log(`Salesforce login attempt ${attempt} failed:`, err && err.message ? err.message : err);
      if (attempt >= MAX_RETRIES) throw err;
      await new Promise((res) => setTimeout(res, RETRY_DELAY));
    }
  }
}

async function queryOrder(conn, orderId) {
  const fields = [
    'Id', 'AccountId', 'EffectiveDate', 'Status', 'TotalAmount', 'Description', 'ContractId', 'OrderNumber',
    'BillingStreet', 'BillingCity', 'BillingState', 'BillingPostalCode', 'BillingCountry',
    'ShippingStreet', 'ShippingCity', 'ShippingState', 'ShippingPostalCode', 'ShippingCountry',
    'CreatedDate', 'LastModifiedDate'
  ];

  const soql = `SELECT ${fields.join(', ')} FROM Order WHERE Id = '${orderId.replace(/'/g, "\\'")}' LIMIT 1`;
  const result = await conn.query(soql);
  if (!result || !result.records || result.records.length === 0) return null;
  const record = result.records[0];
  // jsforce record may include attributes property, remove it
  if (record.attributes) delete record.attributes;
  return record;
}

async function getOrder(orderId) {
  if (!orderId) throw new Error('orderId is required');

  const conn = await connectWithRetry();
  const order = await queryOrder(conn, orderId);
  if (!order) return null;
  return pruneEmpty(order);
}

module.exports = { getOrder };
