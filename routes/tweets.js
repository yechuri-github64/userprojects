try {
  const express = require('express');
  const router = express.Router();
  const tweetsController = require('../controllers/tweetsController');

  router.post('/predict', async (req, res) => {
    try {
      await tweetsController.predict(req, res);
    } catch (err) {
      console.error(' Failed', err);
      res.status(500).json({ error: { message: 'Internal server error', code: 'INTERNAL_ERROR', details: err.message } });
    }
  });

  console.log(' Connected');
  module.exports = router;
} catch (err) {
  console.error(' Failed', err);
  throw err;
}
