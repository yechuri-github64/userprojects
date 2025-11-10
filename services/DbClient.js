const mysql = require('mysql2/promise');

let pool;

function initPool() {
  if (pool) return pool;
  const config = {
    host: process.env.MySQL_HOST || process.env.MYSQL_HOST || 'localhost',
    user: process.env.MySQL_USER || process.env.MYSQL_USER || 'root',
    password: process.env.MySQL_PASSWORD || process.env.MYSQL_PASSWORD || '',
    database: process.env.MySQL_DATABASE || process.env.MYSQL_DATABASE || 'test',
    waitForConnections: true,
    connectionLimit: 10,
    queueLimit: 0
  };
  pool = mysql.createPool(config);
  return pool;
}

async function query(sql, params) {
  const p = initPool();
  const [rows] = await p.execute(sql, params);
  return rows;
}

async function getConnection() {
  const p = initPool();
  return p.getConnection();
}

module.exports = { query, getConnection };
