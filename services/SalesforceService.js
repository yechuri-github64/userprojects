require('dotenv').config();
const jsforce = require('jsforce');

const MAX_RETRIES = parseInt(process.env.SALESFORCE_MAX_RETRIES || '3', 10);
const RETRY_DELAY = parseInt(process.env.SALESFORCE_RETRY_DELAY || '3', 10) * 1000;

const buildConnection = () => {
  const conn = new jsforce.Connection({
    loginUrl: process.env.SALESFORCE_LOGIN_URL || 'https://login.salesforce.com',
    version: process.env.SALESFORCE_API_VERSION || '59.0',
    maxRequest: 120000
  });
  return conn;
};

const loginWithRetry = async (conn) => {
  const username = process.env.SALESFORCE_USERNAME;
  const password = process.env.SALESFORCE_PASSWORD || '';
  const token = process.env.SALESFORCE_SECURITY_TOKEN || '';
  const pwdAndToken = `${password}${token}`;

  let attempt = 0;
  while (true) {
    try {
      attempt++;
      console.log(`Attempting Salesforce login, attempt ${attempt}`);
      await conn.login(username, pwdAndToken);
      console.log('Salesforce login successful');
      return conn;
    } catch (err) {
      console.log('Salesforce login failed', { attempt, error: err && err.message ? err.message : err });
      if (attempt >= MAX_RETRIES) throw new Error('Unable to authenticate to Salesforce');
      await new Promise((res) => setTimeout(res, RETRY_DELAY));
    }
  }
};

exports.updateOrder = async (orderId, orderData) => {
  if (!orderId) throw Object.assign(new Error('orderId is required'), { statusCode: 400 });
  if (!orderData || Object.keys(orderData).length === 0) throw Object.assign(new Error('orderData is required'), { statusCode: 400 });

  const conn = buildConnection();
  await loginWithRetry(conn);

  // Prepare update object: include Id
  const updateObj = Object.assign({ Id: orderId }, orderData);

  try {
    const result = await conn.sobject('Order').update(updateObj);
    if (!result || result.success === false) {
      const errMsg = (result && result.errors) ? JSON.stringify(result.errors) : 'Unknown error from Salesforce update';
      const error = new Error(`Salesforce update failed: ${errMsg}`);
      error.statusCode = 502;
      throw error;
    }
    return result;
  } catch (err) {
    console.log('Error updating Salesforce Order', err && err.message ? err.message : err);
    if (err && err.name === 'INVALID_SESSION_ID') {
      // Attempt one re-login and retry once
      try {
        console.log('Session invalid, re-authenticating');
        await loginWithRetry(conn);
        const retryResult = await conn.sobject('Order').update(updateObj);
        if (!retryResult || retryResult.success === false) {
          const errMsg = (retryResult && retryResult.errors) ? JSON.stringify(retryResult.errors) : 'Unknown error from Salesforce update on retry';
          const error = new Error(`Salesforce update failed on retry: ${errMsg}`);
          error.statusCode = 502;
          throw error;
        }
        return retryResult;
      } catch (innerErr) {
        console.log('Retry failed', innerErr);
        throw innerErr;
      }
    }
    throw err;
  }
};
