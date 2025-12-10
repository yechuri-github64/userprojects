const express = require('express');
const router = express.Router();

try {
  const mysqlRoutes = require('./mysql/mysqlRoutes');
  router.use('/mysql', mysqlRoutes);
  console.log('routes/index Connected');
} catch (err) {
  console.error('routes/index Failed', err);
}

module.exports = router;
