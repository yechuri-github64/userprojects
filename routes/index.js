'use strict';

const express = require('express');
let router;
try {
  router = express.Router();
  console.log(" Connected");
} catch (err) {
  console.error(" Failed", err);
  router = require('express').Router();
}

try {
  const ageRoutes = require('./age');
  router.use('/age', ageRoutes);
  console.log(" Connected");
} catch (err) {
  console.error(" Failed", err);
}

module.exports = router;
