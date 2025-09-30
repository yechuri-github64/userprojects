'use strict';

const axios = require('axios');
const config = require('./index');

let httpClient = null;

async function init() {
  // Initialize and configure an axios instance for the weather provider
  if (!config.WEATHER_API_KEY) {
    // Not throwing to allow app to bootstrap (errors will be surfaced on request)
    console.warn(JSON.stringify({
      timestamp: new Date().toISOString(),
      level: 'warn',
      message: 'WEATHER_API_KEY is not set. Requests to the weather provider will fail.'
    }));
  }

  httpClient = axios.create({
    baseURL: config.WEATHER_API_BASE_URL,
    timeout: 5000,
    headers: {
      'Content-Type': 'application/json'
    }
  });

  // Append API key to every request via params
  httpClient.interceptors.request.use((cfg) => {
    cfg.params = cfg.params || {};
    if (config.WEATHER_API_KEY) cfg.params.appid = config.WEATHER_API_KEY;
    return cfg;
  });

  return Promise.resolve();
}

function getHttpClient() {
  return httpClient;
}

module.exports = { init, getHttpClient };
