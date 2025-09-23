const mysql = require('mysql2/promise');
const env = require('./env');

const host = env.DATABASE_HOST || process.env.DATABASE_HOST || 'localhost';
const user = (env.DATABASE_USER !== undefined) ? env.DATABASE_USER : process.env.DATABASE_USER || '';
const password = (env.DATABASE_PASSWORD !== undefined) ? env.DATABASE_PASSWORD : process.env.DATABASE_PASSWORD || '';
const database = env.DATABASE_NAME || process.env.DATABASE_NAME || '';

const pool = mysql.createPool({
  host,
  user,
  password,
  database,
  waitForConnections: true,
  connectionLimit: 10,
  queueLimit: 0
});

async function query(sql, params) {
  try {
    const [result] = await pool.execute(sql, params);
    return result;
  } catch (err) {
    // rethrow so callers can handle and file can log
    throw err;
  }
}

module.exports = { query, pool };
