const mysql = require('mysql');
const config = require('./config/mysql');

let connection = null;
try {
  connection = mysql.createConnection({
    host: config.host,
    port: config.port,
    user: config.user,
    password: config.password,
    database: config.database
  });

  connection.connect((err) => {
    if (err) {
      console.error('mysql Failed', err);
    } else {
      console.log('mysql Connected');
    }
  });
} catch (err) {
  console.error('mysql Failed', err);
}

module.exports = connection;
