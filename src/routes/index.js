const express = require('express');

let router;
try {
  router = express.Router();

  const ageRoutes = require('./age.routes');
  router.use('/', ageRoutes);

  console.log(' Connected');
} catch (err) {
  console.error(' Failed', err);
  router = express.Router();
}

module.exports = router;
