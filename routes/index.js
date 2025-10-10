const express = require('express');
try {
  const router = express.Router();
  const memesRouter = require('./memes');
  router.use('/', memesRouter);
  module.exports = router;
  console.log(" Connected");
} catch(err) {
  console.error(" Failed", err);
}
