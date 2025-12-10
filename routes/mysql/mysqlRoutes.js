const express = require('express');
const router = express.Router();
const controller = require('../../controllers/mysql/mysqlController');

try {
  router.get('/accounts/:id', controller.getAccount);
  router.post('/accounts', controller.createAccounts);
  router.put('/accounts/:id', controller.updateAccount);
  router.delete('/accounts/:id', controller.deleteAccount);
  console.log('mysqlRoutes Connected');
} catch (err) {
  console.error('mysqlRoutes Failed', err);
}

module.exports = router;
