'use strict';

require('dotenv').config();
const express = require('express');
const cors = require('cors');
const helmet = require('helmet');
const routes = require('./routes');
const config = require('./config');
const connections = require('./config/connections');
const { requestLogger } = require('./middleware/logger');
const { errorHandler } = require('./middleware/errorHandler');

const app = express();

// Initialize external connections (e.g., HTTP clients). Not a DB.
connections.init().catch((err) => {
  // If connections fail, log a structured error and continue to allow routes to return errors later
  console.error(JSON.stringify({
    timestamp: new Date().toISOString(),
    level: 'error',
    message: 'Failed to initialize connections',
    error: err && err.message ? err.message : String(err)
  }));
});

// Middlewares
app.use(helmet());
app.use(cors());
app.use(express.json());
app.use(requestLogger);

// Routes
app.use('/api', routes);

// 404
app.use((req, res) => {
  res.status(404).json({ message: 'Not Found' });
});

// Error handler (structured logging inside)
app.use(errorHandler);

const PORT = config.PORT || 3000;
app.listen(PORT, () => {
  console.log(JSON.stringify({
    timestamp: new Date().toISOString(),
    level: 'info',
    message: `Server started` ,
    port: PORT,
    env: config.NODE_ENV || 'development'
  }));
});
