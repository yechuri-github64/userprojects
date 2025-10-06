const https = require('https');
const { URLSearchParams } = require('url');

function httpGetJson(url) {
  return new Promise((resolve, reject) => {
    try {
      const req = https.get(url, (res) => {
        let data = '';
        res.on('data', chunk => { data += chunk; });
        res.on('end', () => {
          try {
            const statusCode = res.statusCode || 500;
            if (statusCode >= 400) {
              const err = new Error('Upstream service error');
              err.status = statusCode;
              err.code = 'UPSTREAM_ERROR';
              err.details = { url, statusCode, body: data };
              return reject(err);
            }
            const json = data ? JSON.parse(data) : {};
            resolve(json);
          } catch (e) {
            e.code = e.code || 'PARSE_ERROR';
            e.details = { url, raw: data };
            reject(e);
          }
        });
      });
      req.on('error', (err) => {
        err.code = err.code || 'NETWORK_ERROR';
        reject(err);
      });
      req.setTimeout(10000, () => {
        req.destroy(new Error('Request timeout'));
      });
    } catch (err) {
      reject(err);
    }
  });
}

let controller = {};
try {
  controller.getAge = async (req, res, next) => {
    try {
      const name = typeof req.query.name === 'string' ? req.query.name.trim() : '';
      const country_id = typeof req.query.country_id === 'string' ? req.query.country_id.trim() : '';

      if (!name) {
        const err = new Error('Missing required query parameter: name');
        err.status = 400;
        err.code = 'VALIDATION_ERROR';
        err.details = { required: ['name'] };
        throw err;
      }

      const base = process.env.AGE_API_BASE_URL;
      if (!base) {
        const err = new Error('Missing AGE_API_BASE_URL configuration');
        err.status = 500;
        err.code = 'CONFIG_ERROR';
        throw err;
      }

      const params = new URLSearchParams({ name });
      if (country_id) params.append('country_id', country_id);

      const url = `${base}?${params.toString()}`;
      const data = await httpGetJson(url);

      const result = {
        name: data.name,
        age: data.age,
        count: data.count
      };
      if (country_id) {
        result.country_id = country_id;
      }

      res.status(200).json(result);
    } catch (err) {
      console.error(' Failed', err);
      next(err);
    }
  };

  controller.postAge = async (req, res, next) => {
    try {
      const body = req.body || {};
      const name = typeof body.name === 'string' ? body.name.trim() : '';

      if (!name) {
        const err = new Error('Missing required body parameter: name');
        err.status = 400;
        err.code = 'VALIDATION_ERROR';
        err.details = { required: ['name'] };
        throw err;
      }

      const base = process.env.AGE_API_BASE_URL;
      if (!base) {
        const err = new Error('Missing AGE_API_BASE_URL configuration');
        err.status = 500;
        err.code = 'CONFIG_ERROR';
        throw err;
      }

      const params = new URLSearchParams({ name });
      const url = `${base}?${params.toString()}`;

      const data = await httpGetJson(url);

      const result = {
        name: data.name,
        age: data.age,
        count: data.count
      };

      res.status(200).json(result);
    } catch (err) {
      console.error(' Failed', err);
      next(err);
    }
  };

  console.log(' Connected');
} catch (err) {
  console.error(' Failed', err);
}

module.exports = controller;
