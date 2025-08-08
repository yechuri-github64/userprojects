const express = require('express');
const router = express.Router();

// Importing system specific routes
router.use('/postgresql', require('./postgresql/postgresqlRoutes'));
router.use('/mysql', require('./mysql/mysqlRoutes'));

module.exports = router;