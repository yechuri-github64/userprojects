require('dotenv').config();
const express = require('express');
const cors = require('cors');

const app = express();
const PORT = process.env.PORT || 8080;

// Global middleware
app.use(cors());
app.use(express.json());

// Import and initialize backend connections
try {
  const salesforceConnection = require('./connections/salesforce');
  console.log('salesforce connection: Connected');
} catch (err) {
  console.error('salesforce connection: Failed', err);
}

// Import routes (routes/index.js)
try {
  require('./routes')(app);
  console.log('Routes: Connected');
} catch (err) {
  console.error('Routes: Failed', err);
}

app.listen(PORT, () => {
  console.log(`Server listening on port ${PORT}`);
});
