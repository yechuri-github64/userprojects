try {
  require('dotenv').config();
  const express = require('express');
  const cors = require('cors');
  const routes = require('./routes');

  const app = express();

  // Global CORS config: allow all origins and methods
  app.use(cors({ origin: true, methods: ['GET', 'POST', 'PUT', 'DELETE', 'OPTIONS', 'PATCH'] }));
  app.use(express.json());

  app.use('/api', routes);

  const port = process.env.PORT || 8080;
  app.listen(port, () => console.log(' Connected'));
} catch (err) {
  console.error(' Failed', err);
  process.exit(1);
}
