require('dotenv').config();
const jsforce = require('jsforce');
let conn;

async function connect() {
  if (conn && conn.accessToken) return conn;
  const loginUrl = process.env.SALESFORCE_LOGIN_URL || 'https://login.salesforce.com';
  const apiVersion = process.env.SALESFORCE_API_VERSION || '59.0';
  conn = new jsforce.Connection({ loginUrl, version: apiVersion });
  const username = process.env.SALESFORCE_USERNAME;
  const password = process.env.SALESFORCE_PASSWORD || '';
  const token = process.env.SALESFORCE_SECURITY_TOKEN || '';
  if (!username || !password) {
    throw new Error('Salesforce credentials are not set in environment variables');
  }
  try {
    await conn.login(username, password + token);
    console.log('Salesforce connected');
    return conn;
  } catch (err) {
    console.log('Salesforce connection error:', err);
    throw err;
  }
}

async function getOrder(orderId) {
  const c = await connect();
  try {
    const record = await c.sobject('Order').retrieve(orderId);
    return record;
  } catch (err) {
    console.log('getOrder error:', err);
    err.statusCode = err.statusCode || 502;
    throw err;
  }
}

async function updateOrder(orderId, data) {
  const c = await connect();
  try {
    const payload = Object.assign({ Id: orderId }, data);
    const result = await c.sobject('Order').update(payload);
    if (!result.success) {
      const e = new Error('Salesforce update failed');
      e.statusCode = 502;
      throw e;
    }
    return result;
  } catch (err) {
    console.log('updateOrder error:', err);
    err.statusCode = err.statusCode || 502;
    throw err;
  }
}

module.exports = { connect, getOrder, updateOrder };