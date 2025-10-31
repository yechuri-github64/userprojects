const axios = require('axios');

const API_VERSION = process.env.SF_API_VERSION || '56.0';
const INSTANCE_URL = process.env.SF_INSTANCE_URL;
const ACCESS_TOKEN = process.env.SF_ACCESS_TOKEN;

function ensureConfig() {
  if (!INSTANCE_URL || !ACCESS_TOKEN) {
    const err = new Error('Missing Salesforce configuration: SF_INSTANCE_URL and SF_ACCESS_TOKEN are required');
    err.statusCode = 500;
    throw err;
  }
}

function baseUrl() {
  return `${INSTANCE_URL.replace(/\/$/, '')}/services/data/v${API_VERSION}`;
}

function headers() {
  return {
    Authorization: `Bearer ${ACCESS_TOKEN}`,
    'Content-Type': 'application/json'
  };
}

async function getAccount(accountId) {
  ensureConfig();
  try {
    const url = `${baseUrl()}/sobjects/Account/${encodeURIComponent(accountId)}`;
    const resp = await axios.get(url, { headers: headers() });
    return resp.data;
  } catch (err) {
    if (err.response && err.response.status === 404) {
      const e = new Error('Account not found');
      e.statusCode = 404;
      throw e;
    }
    const e = new Error(err.response && err.response.data ? JSON.stringify(err.response.data) : err.message);
    e.statusCode = err.response && err.response.status ? err.response.status : 500;
    throw e;
  }
}

async function createAccount(accountObj) {
  ensureConfig();
  try {
    const url = `${baseUrl()}/sobjects/Account/`;
    const resp = await axios.post(url, accountObj, { headers: headers() });
    // Salesforce returns { id: '...', success: true, errors: [] }
    return resp.data;
  } catch (err) {
    const e = new Error(err.response && err.response.data ? JSON.stringify(err.response.data) : err.message);
    e.statusCode = err.response && err.response.status ? err.response.status : 500;
    throw e;
  }
}

async function updateAccount(accountId, accountObj) {
  ensureConfig();
  try {
    const url = `${baseUrl()}/sobjects/Account/${encodeURIComponent(accountId)}`;
    await axios.patch(url, accountObj, { headers: headers() });
    return { success: true };
  } catch (err) {
    const e = new Error(err.response && err.response.data ? JSON.stringify(err.response.data) : err.message);
    e.statusCode = err.response && err.response.status ? err.response.status : 500;
    throw e;
  }
}

async function deleteAccount(accountId) {
  ensureConfig();
  try {
    const url = `${baseUrl()}/sobjects/Account/${encodeURIComponent(accountId)}`;
    await axios.delete(url, { headers: headers() });
    return { success: true };
  } catch (err) {
    const e = new Error(err.response && err.response.data ? JSON.stringify(err.response.data) : err.message);
    e.statusCode = err.response && err.response.status ? err.response.status : 500;
    throw e;
  }
}

module.exports = {
  getAccount,
  createAccount,
  updateAccount,
  deleteAccount
};
