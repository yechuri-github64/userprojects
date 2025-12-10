const dotenv = require('dotenv');
dotenv.config();

try {
  const config = {
    host: process.env.MYSQL_HOST || 'localhost',
    port: process.env.MYSQL_PORT || '3306',
    user: process.env.MYSQL_USER || 'root',
    password: process.env.MYSQL_PASSWORD || '',
    database: process.env.MYSQL_DATABASE || 'test'
  };
  console.log('connections/config/mysql Connected');
  module.exports = config;
} catch (err) {
  console.error('connections/config/mysql Failed', err);
  module.exports = {};
}
