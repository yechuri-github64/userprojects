'use strict';

const express = require('express');
const router = express.Router();

const weatherRoutes = require('./weather');

router.use('/weather', weatherRoutes);

module.exports = router;
