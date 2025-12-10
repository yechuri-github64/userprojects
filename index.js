const express = require('express');
const cors = require('cors');
require('dotenv').config();

try {
  // Initialize connections (these files attempt to connect on require)
  require('./connections/mysql');
  console.log('Connections initialized');
} catch (err) {
  console.error('Failed to initialize connections', err);
}

try {
  const app = express();
  app.use(cors({ origin: true, methods: ['GET', 'POST', 'PUT', 'DELETE', 'OPTIONS', 'PATCH', 'HEAD'] }));
  app.use(express.json());

  // Routes
  try {
    const routes = require('./routes');
    app.use('/api', routes);
  } catch (err) {
    console.error('Failed to load routes', err);
  }

  const PORT = process.env.PORT || 8080;
  app.listen(PORT, () => {
    console.log(`Server running on port ${PORT}`);
  });
} catch (err) {
  console.error('Failed to start server', err);
}
