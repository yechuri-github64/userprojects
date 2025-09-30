'use strict';

const express = require('express');
const router = express.Router();
const { getWeather } = require('../controllers/weatherController');
const { validateWeatherRequest } = require('../middleware/validateRequest');

// POST /api/weather
router.post('/', validateWeatherRequest, getWeather);

module.exports = router;
