const express = require('express');
const router = express.Router();

const mysqlController = require('../../controllers/mysql/mysqlController');

// Define MySQL-specific routes
router.post('/orders', mysqlController.insertOrders);

module.exports = router;