const axios = require('axios');

const API_VERSION = process.env.SF_API_VERSION || 'v58.0';

if (!process.env.SF_INSTANCE_URL) {
  console.log('Warning: SF_INSTANCE_URL not set. Requests will fail without it.');
}
if (!process.env.SF_ACCESS_TOKEN) {
  console.log('Warning: SF_ACCESS_TOKEN not set. Requests will fail without it.');
}

const getBase = () => {
  const instance = process.env.SF_INSTANCE_URL;
  if (!instance) throw Object.assign(new Error('SF_INSTANCE_URL not configured'), { statusCode: 500 });
  return `${instance.replace(/\/$/, '')}/services/data/${API_VERSION}`;
};

const request = async (method, path, data, config = {}) => {
  const base = getBase();
  const url = path.startsWith('/') ? `${base}${path}` : `${base}/${path}`;
  const token = process.env.SF_ACCESS_TOKEN;
  if (!token) throw Object.assign(new Error('SF_ACCESS_TOKEN not configured'), { statusCode: 500 });

  try {
    const res = await axios({
      method,
      url,
      data,
      headers: Object.assign({
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json'
      }, config.headers || {})
    });
    return res;
  } catch (err) {
    // Normalize Salesforce errors
    if (err.response && err.response.data) {
      const e = new Error('Salesforce API error');
      e.details = err.response.data;
      e.statusCode = err.response.status || 502;
      throw e;
    }
    throw err;
  }
};

module.exports = { request };
