import mysql from "mysql2/promise";

const pool = mysql.createPool({
  host: process.env.MYSQL_HOST || "localhost",
  port: process.env.MYSQL_PORT ? parseInt(process.env.MYSQL_PORT, 10) : 3306,
  user: process.env.MYSQL_USER || "root",
  password: process.env.MYSQL_PASSWORD || "",
  database: process.env.MYSQL_DATABASE || "testdb",
  waitForConnections: true,
  connectionLimit: 10,
  queueLimit: 0,
});

export function getPool() {
  return pool;
}

export async function getConnection() {
  return pool.getConnection();
}

export async function execute(sql, params) {
  const conn = await pool.getConnection();
  try {
    const [rows] = await conn.execute(sql, params);
    return rows;
  } finally {
    try {
      conn.release();
    } catch (e) {
      console.log("connection release error", e);
    }
  }
}
