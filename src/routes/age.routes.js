const express = require('express');
const controller = require('../controllers/age.controller');

let router;
try {
  router = express.Router();

  router.get('/age', controller.getAge);
  router.post('/age', controller.postAge);

  console.log(' Connected');
} catch (err) {
  console.error(' Failed', err);
  router = express.Router();
}

module.exports = router;
