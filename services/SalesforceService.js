require('dotenv').config();
const axios = require('axios');
const querystring = require('querystring');

const CLIENT_ID = process.env.SALESFORCE_CLIENT_ID;
const CLIENT_SECRET = process.env.SALESFORCE_CLIENT_SECRET;
const USERNAME = process.env.SALESFORCE_USERNAME;
const PASSWORD = process.env.SALESFORCE_PASSWORD;
const SECURITY_TOKEN = process.env.SALESFORCE_SECURITY_TOKEN || '';
const LOGIN_URL = process.env.SALESFORCE_LOGIN_URL || 'https://login.salesforce.com';
const API_VERSION = process.env.SALESFORCE_API_VERSION || '59.0';
const TIMEOUT_MS = (parseInt(process.env.SALESFORCE_TIMEOUT_SECONDS || '120', 10) || 120) * 1000;
const ENABLE_LOGGING = (process.env.SALESFORCE_ENABLE_LOGGING || 'false') === 'true';
const MAX_RETRIES = parseInt(process.env.SALESFORCE_MAX_RETRIES || '3', 10) || 3;
const RETRY_DELAY_SECONDS = parseInt(process.env.SALESFORCE_RETRY_DELAY || '3', 10) || 3;

let tokenCache = {
  accessToken: null,
  instanceUrl: process.env.SALESFORCE_INSTANCE_URL || null,
  expiresAt: 0
};

async function requestAccessToken() {
  if (tokenCache.accessToken && Date.now() < tokenCache.expiresAt - 10000 && tokenCache.instanceUrl) {
    if (ENABLE_LOGGING) console.log('Using cached Salesforce token');
    return { accessToken: tokenCache.accessToken, instanceUrl: tokenCache.instanceUrl };
  }

  if (!CLIENT_ID || !CLIENT_SECRET || !USERNAME || !PASSWORD) {
    throw new Error('Salesforce credentials are not fully configured in environment variables');
  }

  const body = querystring.stringify({
    grant_type: 'password',
    client_id: CLIENT_ID,
    client_secret: CLIENT_SECRET,
    username: USERNAME,
    password: `${PASSWORD}${SECURITY_TOKEN}`
  });

  const tokenUrl = `${LOGIN_URL.replace(/\/$/, '')}/services/oauth2/token`;

  const res = await axios.post(tokenUrl, body, {
    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
    timeout: TIMEOUT_MS
  });

  const data = res.data;
  if (!data || !data.access_token) throw new Error('Failed to obtain Salesforce access token');

  tokenCache.accessToken = data.access_token;
  tokenCache.instanceUrl = data.instance_url || tokenCache.instanceUrl || process.env.SALESFORCE_INSTANCE_URL;
  tokenCache.expiresAt = Date.now() + ((data.expires_in || 3600) * 1000);

  if (ENABLE_LOGGING) console.log('Obtained new Salesforce access token');

  return { accessToken: tokenCache.accessToken, instanceUrl: tokenCache.instanceUrl };
}

async function sleep(ms) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

async function sfGet(path, params) {
  const { accessToken, instanceUrl } = await requestAccessToken();
  const url = `${instanceUrl.replace(/\/$/, '')}${path}`;

  for (let attempt = 1; attempt <= MAX_RETRIES; attempt++) {
    try {
      const res = await axios.get(url, {
        headers: { Authorization: `Bearer ${accessToken}` },
        params,
        timeout: TIMEOUT_MS
      });
      return res.data;
    } catch (err) {
      const status = err && err.response && err.response.status;
      if (ENABLE_LOGGING) console.log(`Salesforce GET attempt ${attempt} failed`, status || err.message);
      // If unauthorized, clear token and retry once
      if (status === 401) {
        tokenCache.accessToken = null;
        tokenCache.expiresAt = 0;
      }
      if (attempt === MAX_RETRIES) throw err;
      await sleep(RETRY_DELAY_SECONDS * 1000);
    }
  }
}

async function getOrderById(orderId) {
  if (!orderId) throw new Error('orderId is required');
  const soql = `SELECT Id, Name, AccountId, EffectiveDate, Status, TotalAmount FROM Order WHERE Id='${orderId}' LIMIT 1`;
  const encoded = encodeURIComponent(soql);
  const path = `/services/data/v${API_VERSION}/query?q=${encoded}`;
  const result = await sfGet(path);
  if (!result || !result.records || result.records.length === 0) return null;
  return result.records[0];
}

async function getAllOrders() {
  const soql = `SELECT Id, Name, AccountId, EffectiveDate, Status, TotalAmount FROM Order ORDER BY EffectiveDate DESC LIMIT 200`;
  const encoded = encodeURIComponent(soql);
  const path = `/services/data/v${API_VERSION}/query?q=${encoded}`;
  const result = await sfGet(path);
  return result && result.records ? result.records : [];
}

module.exports = {
  getOrderById,
  getAllOrders
};