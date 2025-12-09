require('dotenv').config();
const jsforce = require('jsforce');

const MAX_RETRIES = parseInt(process.env.SALESFORCE_MAX_RETRIES || '3', 10);
const RETRY_DELAY = parseInt(process.env.SALESFORCE_RETRY_DELAY || '3', 10) * 1000;
const TIMEOUT_MS = (parseInt(process.env.SALESFORCE_TIMEOUT_SECONDS || '120', 10)) * 1000;

const connect = async () => {
  const connOptions = {
    loginUrl: process.env.SALESFORCE_LOGIN_URL || 'https://login.salesforce.com',
    version: process.env.SALESFORCE_API_VERSION || '59.0'
  };

  // If an explicit instance URL is provided, set it after login
  const conn = new jsforce.Connection(connOptions);
  conn._timeout = TIMEOUT_MS;

  const username = process.env.SALESFORCE_USERNAME;
  const password = process.env.SALESFORCE_PASSWORD;
  const token = process.env.SALESFORCE_SECURITY_TOKEN || '';

  if (!username || !password) {
    throw { statusCode: 500, message: 'Salesforce credentials not configured in environment' };
  }

  let attempt = 0;
  while (attempt < MAX_RETRIES) {
    attempt += 1;
    try {
      await new Promise((resolve, reject) => {
        conn.login(username, password + token, (err, userInfo) => {
          if (err) return reject(err);
          return resolve(userInfo);
        });
      });

      // If instance URL specified, override (useful for named instances or sandbox)
      if (process.env.SALESFORCE_INSTANCE_URL) {
        conn.instanceUrl = process.env.SALESFORCE_INSTANCE_URL;
      }

      if (process.env.SALESFORCE_ENABLE_LOGGING === 'true') {
        console.log('Connected to Salesforce', { instanceUrl: conn.instanceUrl, userId: conn.userId });
      }

      return conn;
    } catch (err) {
      console.log(`Salesforce connection attempt ${attempt} failed`, err && err.message ? err.message : err);
      if (attempt >= MAX_RETRIES) {
        throw { statusCode: 502, message: 'Failed to connect to Salesforce after retries' };
      }
      await new Promise((res) => setTimeout(res, RETRY_DELAY));
    }
  }
};

exports.getAccount = async (id) => {
  if (!id) throw { statusCode: 400, message: 'Account id is required' };
  const conn = await connect();
  try {
    const record = await conn.sobject('Account').retrieve(id);
    return record;
  } catch (err) {
    console.log('getAccount error', err && err.message ? err.message : err);
    if (err && err.errorCode === 'NOT_FOUND') throw { statusCode: 404, message: 'Account not found' };
    throw { statusCode: 502, message: 'Error retrieving account from Salesforce' };
  }
};

exports.listAccounts = async (q) => {
  const conn = await connect();
  try {
    const soql = q || 'SELECT Id, Name, Industry FROM Account LIMIT 100';
    const query = q ? q : soql;
    const result = await conn.query(query);
    return { totalSize: result.totalSize, records: result.records };
  } catch (err) {
    console.log('listAccounts error', err && err.message ? err.message : err);
    throw { statusCode: 502, message: 'Error querying accounts from Salesforce' };
  }
};

exports.createAccount = async (data) => {
  if (!data || typeof data !== 'object') throw { statusCode: 400, message: 'Invalid account payload' };
  const conn = await connect();
  try {
    const result = await conn.sobject('Account').create(data);
    if (!result.success) {
      console.log('createAccount failed', result);
      throw { statusCode: 502, message: 'Failed to create account in Salesforce' };
    }
    return { id: result.id, success: true };
  } catch (err) {
    console.log('createAccount error', err && err.message ? err.message : err);
    throw { statusCode: 502, message: 'Error creating account in Salesforce' };
  }
};

exports.updateAccount = async (id, data) => {
  if (!id) throw { statusCode: 400, message: 'Account id is required' };
  if (!data || typeof data !== 'object') throw { statusCode: 400, message: 'Invalid account payload' };
  const conn = await connect();
  try {
    const record = Object.assign({}, data, { Id: id });
    const result = await conn.sobject('Account').update(record);
    if (!result.success) {
      console.log('updateAccount failed', result);
      throw { statusCode: 502, message: 'Failed to update account in Salesforce' };
    }
    return { id: result.id, success: true };
  } catch (err) {
    console.log('updateAccount error', err && err.message ? err.message : err);
    if (err && err.errorCode === 'NOT_FOUND') throw { statusCode: 404, message: 'Account not found' };
    throw { statusCode: 502, message: 'Error updating account in Salesforce' };
  }
};

exports.deleteAccount = async (id) => {
  if (!id) throw { statusCode: 400, message: 'Account id is required' };
  const conn = await connect();
  try {
    const result = await conn.sobject('Account').destroy(id);
    if (!result.success) {
      console.log('deleteAccount failed', result);
      throw { statusCode: 502, message: 'Failed to delete account in Salesforce' };
    }
    return { id: result.id, success: true };
  } catch (err) {
    console.log('deleteAccount error', err && err.message ? err.message : err);
    if (err && err.errorCode === 'NOT_FOUND') throw { statusCode: 404, message: 'Account not found' };
    throw { statusCode: 502, message: 'Error deleting account in Salesforce' };
  }
};
