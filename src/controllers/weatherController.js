'use strict';

const connections = require('../config/connections');
const config = require('../config');
const { degToCompass } = require('../utils/formatters');

async function getWeather(req, res, next) {
  try {
    const { city, units, lang } = req.body;

    const axiosInstance = connections.getHttpClient();
    if (!axiosInstance) {
      // No HTTP client available
      const err = new Error('Weather HTTP client not initialized');
      err.status = 500;
      throw err;
    }

    const params = { q: city };
    if (units) params.units = units;
    if (lang) params.lang = lang;
    // API key will be appended by the axios instance defaults (connections.init sets it)

    const resp = await axiosInstance.get('/data/2.5/weather', { params });
    const data = resp.data;

    const output = {
      city: data.name || city,
      country: data.sys && data.sys.country ? data.sys.country : '',
      temperature: (data.main && typeof data.main.temp === 'number') ? data.main.temp : null,
      units: units || (config.DEFAULT_UNITS || ''),
      condition: Array.isArray(data.weather) && data.weather[0] && data.weather[0].description ? data.weather[0].description : '',
      humidity: data.main && typeof data.main.humidity === 'number' ? data.main.humidity : null,
      windSpeed: data.wind && typeof data.wind.speed === 'number' ? data.wind.speed : null,
      windDirection: data.wind && typeof data.wind.deg === 'number' ? degToCompass(data.wind.deg) : '',
      timestamp: data.dt ? new Date(data.dt * 1000).toISOString() : new Date().toISOString()
    };

    res.json(output);
  } catch (err) {
    // Attach status if not present
    if (!err.status) err.status = (err.response && err.response.status) ? err.response.status : 500;
    // Attach helpful message if we received a response from the weather provider
    if (err.response && err.response.data) {
      err.meta = { provider: 'weather', providerResponse: err.response.data };
    }
    next(err);
  }
}

module.exports = { getWeather };
