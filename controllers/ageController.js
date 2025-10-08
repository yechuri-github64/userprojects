'use strict';

const https = require('https');

function buildError(status, code, message, details) {
  const err = new Error(message || code);
  err.status = status;
  err.code = code;
  if (details !== undefined) err.details = details;
  return err;
}

function fetchJson(url) {
  return new Promise((resolve, reject) => {
    try {
      https
        .get(url, (res) => {
          let data = '';
          res.on('data', (chunk) => {
            data += chunk;
          });
          res.on('end', () => {
            try {
              const parsed = JSON.parse(data || '{}');
              resolve({ statusCode: res.statusCode || 200, data: parsed });
              console.log(" Connected");
            } catch (e) {
              console.error(" Failed", e);
              reject(buildError(502, 'EXTERNAL_JSON_PARSE_ERROR', 'Invalid JSON from external service', { url }));
            }
          });
        })
        .on('error', (err) => {
          console.error(" Failed", err);
          reject(buildError(502, 'EXTERNAL_REQUEST_ERROR', 'External service request failed', { url }));
        });
    } catch (err) {
      console.error(" Failed", err);
      reject(buildError(500, 'REQUEST_CREATION_ERROR', 'Failed to create request to external service', { url }));
    }
  });
}

async function getAge(req, res, next) {
  try {
    const name = typeof req.query.name === 'string' ? req.query.name.trim() : '';
    if (!name) {
      throw buildError(400, 'VALIDATION_ERROR', 'Parameter "name" is required in query', { location: 'query', field: 'name' });
    }
    const baseUrl = process.env.AGE_SERVICE_URL || 'https://api.agify.io';
    const url = `${baseUrl}?name=${encodeURIComponent(name)}`;
    const response = await fetchJson(url);
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw buildError(response.statusCode, 'EXTERNAL_SERVICE_ERROR', 'External service returned an error status', { url, statusCode: response.statusCode });
    }
    console.log(" Connected");
    return res.status(200).json({ success: true, data: response.data });
  } catch (err) {
    console.error(" Failed", err);
    return next(err);
  }
}

async function postAge(req, res, next) {
  try {
    const nameHeader = typeof req.headers.name === 'string' ? req.headers.name.trim() : '';
    const nameQuery = typeof req.query.name === 'string' ? req.query.name.trim() : '';
    const nameBody = req.body && typeof req.body.name === 'string' ? req.body.name.trim() : '';
    const name = nameBody || nameQuery || nameHeader;
    if (!name) {
      throw buildError(400, 'VALIDATION_ERROR', 'Parameter "name" is required in body, query, or headers', { acceptedLocations: ['body', 'query', 'headers'], field: 'name' });
    }
    const baseUrl = process.env.AGE_SERVICE_URL || 'https://api.agify.io';
    const url = `${baseUrl}?name=${encodeURIComponent(name)}`;
    const response = await fetchJson(url);
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw buildError(response.statusCode, 'EXTERNAL_SERVICE_ERROR', 'External service returned an error status', { url, statusCode: response.statusCode });
    }
    console.log(" Connected");
    return res.status(200).json({ success: true, data: response.data });
  } catch (err) {
    console.error(" Failed", err);
    return next(err);
  }
}

try {
  console.log(" Connected");
} catch (e) {
  console.error(" Failed", e);
}

module.exports = { getAge, postAge };
