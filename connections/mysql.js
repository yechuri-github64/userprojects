const mysql = require('mysql2/promise');
const config = require('./config/mysql');

let mysqlConnection;
(async () => {
  try {
    mysqlConnection = await mysql.createPool(config);
    console.log('✅ Connected to MySQL');
  } catch (error) {
    console.error('❌ Failed to connect to MySQL', error);
  }
})();

module.exports = mysqlConnection;