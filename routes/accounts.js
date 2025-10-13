try {
  const express = require('express');
  const router = express.Router();
  const controller = require('../controllers/accountsController');

  router.get('/:id', controller.getAccount);
  router.post('/', controller.createAccounts);
  router.put('/:id', controller.updateAccount);
  router.delete('/:id', controller.deleteAccount);

  console.log("Connected");
  module.exports = router;
} catch (err) {
  console.error("Failed", err);
  const express = require('express');
  module.exports = express.Router();
}
