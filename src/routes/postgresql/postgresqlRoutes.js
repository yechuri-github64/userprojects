const express = require('express');
const router = express.Router();

const postgresqlController = require('../../controllers/postgresql/postgresqlController');

// Define PostgreSQL-specific routes
router.get('/accounts', postgresqlController.getAccounts);

module.exports = router;