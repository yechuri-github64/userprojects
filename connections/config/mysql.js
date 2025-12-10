require('dotenv').config();

try {
  const cfg = {
    host: process.env.MYSQL_HOST || 'localhost',
    user: process.env.MYSQL_USER || 'root',
    password: process.env.MYSQL_PASSWORD || '',
    database: process.env.MYSQL_DATABASE || 'test',
    port: process.env.MYSQL_PORT ? parseInt(process.env.MYSQL_PORT, 10) : 3306
  };

  module.exports = cfg;
} catch (err) {
  console.error('Failed', err);
  module.exports = {};
}
