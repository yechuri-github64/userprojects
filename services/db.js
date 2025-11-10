const mysql = require('mysql2/promise');

let pool = null;

const getPool = () => {
  if (pool) return pool;
  const config = {
    host: process.env.DB_HOST || '127.0.0.1',
    user: process.env.DB_USER || 'root',
    password: process.env.DB_PASSWORD || '',
    database: process.env.DB_NAME || 'test',
    port: process.env.DB_PORT ? parseInt(process.env.DB_PORT, 10) : 3306,
    waitForConnections: true,
    connectionLimit: 10,
    queueLimit: 0
  };
  pool = mysql.createPool(config);
  return pool;
};

const execute = async (sql, params) => {
  const p = getPool();
  const [rows] = await p.execute(sql, params);
  return rows;
};

module.exports = { getPool, execute };