const express = require('express');
const cors = require('cors');
const dotenv = require('dotenv');
dotenv.config();
const app = express();
app.use(cors({ origin: '*', methods: ['GET','POST','PUT','DELETE','OPTIONS'] }));
app.use(express.json());

try {
  // initialize connections
  require('./connections/mysql');
  console.log('mysql Connected');
} catch (err) {
  console.error('mysql Failed', err);
}

try {
  const routes = require('./routes');
  app.use('/api', routes);
  console.log('Routes Connected');
} catch (err) {
  console.error('Routes Failed', err);
}

const PORT = process.env.PORT || 8080;
try {
  app.listen(PORT, () => {
    console.log('Server listening on port', PORT);
  });
  console.log('Server Connected');
} catch (err) {
  console.error('Server Failed', err);
}
