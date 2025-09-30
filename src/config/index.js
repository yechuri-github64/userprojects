'use strict';

const dotenv = require('dotenv');

// Already loaded in index.js, but safe to call here if required directly
dotenv.config();

module.exports = {
  PORT: process.env.PORT ? Number(process.env.PORT) : 3000,
  WEATHER_API_KEY: process.env.WEATHER_API_KEY || '',
  WEATHER_API_BASE_URL: process.env.WEATHER_API_BASE_URL || 'https://api.openweathermap.org',
  NODE_ENV: process.env.NODE_ENV || 'development',
  DEFAULT_UNITS: process.env.DEFAULT_UNITS || 'metric'
};
