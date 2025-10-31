const fetch = require('node-fetch');

function isEmptyValue(val) {
  if (val === null || val === undefined) return true;
  if (typeof val === 'string' && val.trim() === '') return true;
  if (Array.isArray(val) && val.length === 0) return true;
  if (typeof val === 'object') {
    return Object.keys(val).length === 0;
  }
  return false;
}

function cleanObject(input) {
  if (input === null || input === undefined) return undefined;

  if (Array.isArray(input)) {
    const arr = input
      .map((item) => cleanObject(item))
      .filter((item) => !isEmptyValue(item));
    return arr.length ? arr : undefined;
  }

  if (typeof input === 'object') {
    const out = {};
    for (const [k, v] of Object.entries(input)) {
      const cleaned = cleanObject(v);
      if (!isEmptyValue(cleaned)) {
        out[k] = cleaned;
      }
    }
    return Object.keys(out).length ? out : undefined;
  }

  // primitives
  if (typeof input === 'string') {
    return input.trim() === '' ? undefined : input;
  }

  return input;
}

async function getAccountById(accountId) {
  if (!accountId) throw new Error('accountId is required');

  const instanceUrl = process.env.SF_INSTANCE_URL;
  const accessToken = process.env.SF_ACCESS_TOKEN;
  const apiVersion = process.env.SF_API_VERSION || 'v57.0';

  if (!instanceUrl || !accessToken) {
    throw new Error('Salesforce credentials missing. Set SF_INSTANCE_URL and SF_ACCESS_TOKEN environment variables.');
  }

  const base = instanceUrl.replace(/\/+$/, '');
  const url = `${base}/services/data/${apiVersion}/sobjects/Account/${encodeURIComponent(accountId)}`;

  console.log('Fetching Salesforce Account', { url });

  const res = await fetch(url, {
    method: 'GET',
    headers: {
      Authorization: `Bearer ${accessToken}`,
      Accept: 'application/json'
    }
  });

  const text = await res.text();
  let data = {};
  try {
    data = text ? JSON.parse(text) : {};
  } catch (parseErr) {
    console.log('Failed to parse Salesforce response as JSON', parseErr);
    throw new Error('Invalid JSON from Salesforce');
  }

  if (res.status === 404) {
    throw new Error('Account not found');
  }

  if (!res.ok) {
    const msg = data && data.message ? data.message : res.statusText || 'Salesforce API error';
    throw new Error(`Salesforce error ${res.status}: ${msg}`);
  }

  const cleaned = cleanObject(data) || {};
  return cleaned;
}

module.exports = {
  getAccountById
};
