try {
  const express = require('express');
  const router = express.Router();
  const tweetsRouter = require('./tweets');

  // Combine individual route files
  router.use('/tweets', tweetsRouter);

  console.log(' Connected');
  module.exports = router;
} catch (err) {
  console.error(' Failed', err);
  throw err;
}
