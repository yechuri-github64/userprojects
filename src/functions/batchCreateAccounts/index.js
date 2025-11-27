const pool = require("../../helpers/mysql/connection");
const checkApiKey = require("../../helpers/auth");
const responses = require("../../helpers/responses");

module.exports = async function (context, req) {
  context.log("HTTP trigger - Batch Create Accounts");
  if (!checkApiKey(req)) {
    context.log("Unauthorized request to batch create accounts");
    context.res = {
      status: 401,
      headers: { "Content-Type": "application/json" },
      body: responses.error("Unauthorized", "unauthorized"),
    };
    return;
  }

  const accounts = (req.body && req.body.accounts) || [];
  if (!Array.isArray(accounts) || accounts.length === 0) {
    context.res = {
      status: 400,
      headers: { "Content-Type": "application/json" },
      body: responses.error("accounts array is required", "invalid_input"),
    };
    return;
  }

  let connection;
  try {
    connection = await pool.getConnection();
    await connection.beginTransaction();

    const inserted = [];
    for (const acc of accounts) {
      const name = acc.name && String(acc.name).trim();
      const email = acc.email && String(acc.email).trim();
      if (!name || !email) {
        throw new Error("Each account must have name and email");
      }
      const [result] = await connection.execute(
        "INSERT INTO accounts (name, email) VALUES (?, ?)",
        [name, email]
      );
      const id = result.insertId;
      const [rows] = await connection.execute(
        "SELECT id, name, email, created_at FROM accounts WHERE id = ?",
        [id]
      );
      inserted.push(rows[0]);
    }

    await connection.commit();
    context.res = {
      status: 201,
      headers: { "Content-Type": "application/json" },
      body: responses.success(inserted),
    };
  } catch (err) {
    if (connection) {
      try {
        await connection.rollback();
      } catch (e) {
        context.log("Rollback failed", e);
      }
    }
    context.log.error("Batch create failed", err);
    context.res = {
      status: 500,
      headers: { "Content-Type": "application/json" },
      body: responses.error("Batch create failed", "db_error"),
    };
  } finally {
    if (connection) connection.release();
  }
};
