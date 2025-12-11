try {
  const express = require('express');
  const router = express.Router();
  const convertController = require('../controllers/convertController');

  // Combine individual route files here (single route for conversion)
  router.get('/convert', convertController.convert);
  router.post('/convert', convertController.convert);

  console.log('Connected');
  module.exports = router;
} catch (err) {
  console.error('Failed', err);
  const express = require('express');
  module.exports = express.Router();
}
