'use strict';

try {
  const path = require('path');
  const express = require('express');
  const cors = require('cors');
  const dotenv = require('dotenv');

  dotenv.config();

  const app = express();

  app.use(cors({
    origin: '*',
    methods: ['GET','POST','PUT','PATCH','DELETE','OPTIONS'],
    allowedHeaders: ['Content-Type','Authorization','Accept','Origin','X-Requested-With'],
    exposedHeaders: ['Content-Type'],
    credentials: false,
    preflightContinue: false,
    optionsSuccessStatus: 204
  }));
  app.options('*', cors());

  app.use(express.json());

  const routes = require('./routes');
  app.use('/', routes);

  app.use((req, res) => {
    res.status(404).json({
      success: false,
      error: {
        code: 'NOT_FOUND',
        message: 'Route not found',
        details: { method: req.method, path: req.originalUrl }
      }
    });
  });

  const errorHandler = require('./middleware/errorHandler');
  app.use(errorHandler);

  const PORT = process.env.PORT || 3000;
  app.listen(PORT, () => {
    console.log(' Connected');
  });
} catch (err) {
  console.error(' Failed', err);
}
