'use strict';

try {
  console.log(" Connected");
} catch (e) {
  console.error(" Failed", e);
}

const express = require('express');
const cors = require('cors');
const dotenv = require('dotenv');

try {
  dotenv.config();
  console.log(" Connected");
} catch (e) {
  console.error(" Failed", e);
}

let app;
try {
  app = express();
  console.log(" Connected");
} catch (err) {
  console.error(" Failed", err);
  throw err;
}

try {
  app.use(cors({
    origin: '*',
    methods: ['GET', 'POST', 'PUT', 'PATCH', 'DELETE', 'OPTIONS'],
    allowedHeaders: ['Content-Type', 'Authorization', 'name']
  }));
  app.use(express.json());
  console.log(" Connected");
} catch (err) {
  console.error(" Failed", err);
}

try {
  const routes = require('./routes');
  app.use('/api', routes);
  console.log(" Connected");
} catch (err) {
  console.error(" Failed", err);
}

const errorHandler = require('./middleware/errorHandler');

try {
  app.use(errorHandler);
  console.log(" Connected");
} catch (err) {
  console.error(" Failed", err);
}

const PORT = process.env.PORT || 8080;

try {
  app.listen(PORT, () => {
    console.log(" Connected");
  });
} catch (err) {
  console.error(" Failed", err);
}
