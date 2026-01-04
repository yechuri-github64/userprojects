const express = require('express');
const router = express.Router();
const controller = require('../../controllers/salesforce/salesforceController');

// Create order
router.post('/', async (req, res) => {
  try {
    await controller.createOrder(req, res);
  } catch (err) {
    console.error('salesforceRoutes POST /: Failed', err);
    res.status(500).json({ error: { message: 'Internal server error', details: err.message } });
  }
});

// Get all orders
router.get('/', async (req, res) => {
  try {
    await controller.getOrders(req, res);
  } catch (err) {
    console.error('salesforceRoutes GET /: Failed', err);
    res.status(500).json({ error: { message: 'Internal server error', details: err.message } });
  }
});

// Get order by id
router.get('/:id', async (req, res) => {
  try {
    await controller.getOrderById(req, res);
  } catch (err) {
    console.error('salesforceRoutes GET /:id Failed', err);
    res.status(500).json({ error: { message: 'Internal server error', details: err.message } });
  }
});

// Update order
router.put('/:id', async (req, res) => {
  try {
    await controller.updateOrder(req, res);
  } catch (err) {
    console.error('salesforceRoutes PUT /:id Failed', err);
    res.status(500).json({ error: { message: 'Internal server error', details: err.message } });
  }
});

// Delete order
router.delete('/:id', async (req, res) => {
  try {
    await controller.deleteOrder(req, res);
  } catch (err) {
    console.error('salesforceRoutes DELETE /:id Failed', err);
    res.status(500).json({ error: { message: 'Internal server error', details: err.message } });
  }
});

module.exports = router;
