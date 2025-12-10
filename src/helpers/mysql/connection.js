const mysql = require("mysql2/promise");

const pool = mysql.createPool({
  host: process.env.MYSQL_HOST || "localhost",
  port: process.env.MYSQL_PORT ? parseInt(process.env.MYSQL_PORT, 10) : 3306,
  user: process.env.MYSQL_USER || "root",
  password: process.env.MYSQL_PASSWORD || "",
  database: process.env.MYSQL_DATABASE || "test",
  waitForConnections: true,
  connectionLimit: 10,
  queueLimit: 0,
});

async function execute(query, params) {
  const [rows] = await pool.execute(query, params);
  return rows;
}

module.exports = {
  pool,
  execute,
};
