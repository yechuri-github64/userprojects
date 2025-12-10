const mysql = require('mysql');
const config = require('./config/mysql');

let pool;
try {
  pool = mysql.createPool({
    connectionLimit: 10,
    host: config.host,
    user: config.user,
    password: config.password,
    database: config.database,
    port: config.port
  });

  // Test connection
  pool.getConnection((err, connection) => {
    if (err) {
      console.error('Failed', err);
    } else {
      console.log('Connected');
      connection.release();
    }
  });
} catch (err) {
  console.error('Failed', err);
}

module.exports = pool;
