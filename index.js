require('dotenv').config();
const express = require('express');
const cors = require('cors');
const routes = require('./routes');
try {
  const app = express();

  // Global CORS config: allow all origins and all methods
  app.use(cors());
  app.options('*', cors());

  app.use(express.json());
  app.use(express.urlencoded({ extended: true }));

  app.use('/api', routes);

  const PORT = process.env.PORT || 8080;
  app.listen(PORT, () => {
    console.log('Connected');
    console.log(`Server listening on port ${PORT}`);
  });
} catch (err) {
  console.error('Failed', err);
}
