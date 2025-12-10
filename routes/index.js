const express = require('express');
const router = express.Router();

try {
  // Mount mysql routes
  try {
    const mysqlRoutes = require('./mysql/mysqlRoutes');
    router.use('/mysql', mysqlRoutes);
  } catch (err) {
    console.error('Failed to load mysql routes', err);
  }
} catch (err) {
  console.error('Failed in routes index', err);
}

module.exports = router;
