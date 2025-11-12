require('dotenv').config();
const axios = require('axios');
const utils = require('./utils');

const MAX_RETRIES = parseInt(process.env.SALESFORCE_MAX_RETRIES || '3', 10);
const RETRY_DELAY = parseInt(process.env.SALESFORCE_RETRY_DELAY || '3', 10);
const TIMEOUT = (parseInt(process.env.SALESFORCE_TIMEOUT_SECONDS || '120', 10)) * 1000;

async function getAccessToken() {
  const loginUrl = process.env.SALESFORCE_LOGIN_URL || 'https://login.salesforce.com';
  const tokenUrl = `${loginUrl}/services/oauth2/token`;
  const params = new URLSearchParams();
  params.append('grant_type', 'password');
  params.append('client_id', process.env.SALESFORCE_CLIENT_ID || '');
  params.append('client_secret', process.env.SALESFORCE_CLIENT_SECRET || '');
  params.append('username', process.env.SALESFORCE_USERNAME || '');
  const pwd = (process.env.SALESFORCE_PASSWORD || '') + (process.env.SALESFORCE_SECURITY_TOKEN || '');
  params.append('password', pwd);

  console.log('Requesting Salesforce access token');
  const fn = async () => {
    const res = await axios.post(tokenUrl, params.toString(), { headers: { 'Content-Type': 'application/x-www-form-urlencoded' }, timeout: TIMEOUT });
    return res.data;
  };

  return await utils.retry(fn, MAX_RETRIES, RETRY_DELAY);
}

async function createOrder(order) {
  if (!order || typeof order !== 'object') throw Object.assign(new Error('Invalid order data'), { statusCode: 400 });
  const tokenData = await getAccessToken();
  const instanceUrl = process.env.SALESFORCE_INSTANCE_URL || tokenData.instance_url;
  const apiVersion = process.env.SALESFORCE_API_VERSION || '59.0';
  const endpoint = `${instanceUrl}/services/data/v${apiVersion}/sobjects/Order/`;

  console.log('Creating Order at', endpoint);
  const fn = async () => {
    const res = await axios.post(endpoint, order, { headers: { Authorization: `Bearer ${tokenData.access_token}`, 'Content-Type': 'application/json' }, timeout: TIMEOUT });
    return res.data;
  };

  const created = await utils.retry(fn, MAX_RETRIES, RETRY_DELAY);
  return created;
}

module.exports = { getAccessToken, createOrder };
