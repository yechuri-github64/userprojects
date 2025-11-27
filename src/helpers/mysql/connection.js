const mysql = require("mysql2/promise");

const pool = mysql.createPool({
  host: process.env.MYSQL_HOST || "127.0.0.1",
  port: parseInt(process.env.MYSQL_PORT || "3306", 10),
  user: process.env.MYSQL_USER || "root",
  password: process.env.MYSQL_PASSWORD || "",
  database: process.env.MYSQL_DATABASE || "accounts",
  waitForConnections: true,
  connectionLimit: 10,
});

async function query(sql, params) {
  const [rows] = await pool.execute(sql, params);
  return rows;
}

async function createAccount(payload) {
  const [res] = await pool.execute(
    "INSERT INTO accounts (name, email, address) VALUES (?, ?, ?)",
    [payload.name || null, payload.email || null, payload.address || null]
  );
  return {
    id: res.insertId,
    name: payload.name || null,
    email: payload.email || null,
    address: payload.address || null,
  };
}

async function batchCreate(accounts) {
  const conn = await pool.getConnection();
  try {
    await conn.beginTransaction();
    const results = [];
    for (const a of accounts) {
      const [res] = await conn.execute(
        "INSERT INTO accounts (name, email, address) VALUES (?, ?, ?)",
        [a.name || null, a.email || null, a.address || null]
      );
      results.push({
        id: res.insertId,
        name: a.name || null,
        email: a.email || null,
        address: a.address || null,
      });
    }
    await conn.commit();
    return results;
  } catch (err) {
    await conn.rollback();
    throw err;
  } finally {
    conn.release();
  }
}

async function listAccounts() {
  return query("SELECT id, name, email, address FROM accounts", []);
}

async function getAccount(id) {
  const rows = await query(
    "SELECT id, name, email, address FROM accounts WHERE id = ?",
    [id]
  );
  return rows[0];
}

async function updateAccount(id, payload) {
  const fields = [];
  const params = [];
  if (payload.name !== undefined) {
    fields.push("name = ?");
    params.push(payload.name);
  }
  if (payload.email !== undefined) {
    fields.push("email = ?");
    params.push(payload.email);
  }
  if (payload.address !== undefined) {
    fields.push("address = ?");
    params.push(payload.address);
  }
  if (fields.length === 0) {
    return getAccount(id);
  }
  params.push(id);
  await pool.execute(
    `UPDATE accounts SET ${fields.join(", ")} WHERE id = ?`,
    params
  );
  return getAccount(id);
}

async function deleteAccount(id) {
  await pool.execute("DELETE FROM accounts WHERE id = ?", [id]);
  return true;
}

module.exports = {
  pool,
  query,
  createAccount,
  batchCreate,
  listAccounts,
  getAccount,
  updateAccount,
  deleteAccount,
};
