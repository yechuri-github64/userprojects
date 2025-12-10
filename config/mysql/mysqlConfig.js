const config = require('../../connections/config/mysql');

try {
  console.log('config/mysql/mysqlConfig Connected');
} catch (err) {
  console.error('config/mysql/mysqlConfig Failed', err);
}

module.exports = config;
