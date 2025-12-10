const express = require('express');
const router = express.Router();

try {
  const controller = require('../../controllers/mysql/mysqlController');

  router.get('/accounts/:id', async (req, res) => {
    try {
      await controller.getAccount(req, res);
    } catch (err) {
      console.error('Failed', err);
      res.status(500).json({ success: false, error: { message: err.message } });
    }
  });

  router.post('/accounts', async (req, res) => {
    try {
      await controller.createAccounts(req, res);
    } catch (err) {
      console.error('Failed', err);
      res.status(500).json({ success: false, error: { message: err.message } });
    }
  });

  router.put('/accounts/:id', async (req, res) => {
    try {
      await controller.updateAccount(req, res);
    } catch (err) {
      console.error('Failed', err);
      res.status(500).json({ success: false, error: { message: err.message } });
    }
  });

  router.delete('/accounts/:id', async (req, res) => {
    try {
      await controller.deleteAccount(req, res);
    } catch (err) {
      console.error('Failed', err);
      res.status(500).json({ success: false, error: { message: err.message } });
    }
  });
} catch (err) {
  console.error('Failed to initialize mysql routes', err);
}

module.exports = router;
