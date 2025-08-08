const { Client } = require('pg');
const config = require('./config/postgresql');

const pgClient = new Client(config);
(async () => {
  try {
    await pgClient.connect();
    console.log('✅ Connected to PostgreSQL');
  } catch (error) {
    console.error('❌ Failed to connect to PostgreSQL', error);
  }
})();

module.exports = pgClient;