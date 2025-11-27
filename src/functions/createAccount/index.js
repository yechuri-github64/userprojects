const pool = require("../../helpers/mysql/connection");
const checkApiKey = require("../../helpers/auth");
const responses = require("../../helpers/responses");

module.exports = async function (context, req) {
  context.log("HTTP trigger - Create Account");
  if (!checkApiKey(req)) {
    context.log("Unauthorized request to create account");
    context.res = {
      status: 401,
      headers: { "Content-Type": "application/json" },
      body: responses.error("Unauthorized", "unauthorized"),
    };
    return;
  }

  try {
    const body = req.body || {};
    const name = body.name && String(body.name).trim();
    const email = body.email && String(body.email).trim();

    if (!name || !email) {
      context.res = {
        status: 400,
        headers: { "Content-Type": "application/json" },
        body: responses.error("name and email are required", "invalid_input"),
      };
      return;
    }

    const [result] = await pool.execute(
      "INSERT INTO accounts (name, email) VALUES (?, ?)",
      [name, email]
    );
    const insertId = result.insertId;
    const [rows] = await pool.execute(
      "SELECT id, name, email, created_at FROM accounts WHERE id = ?",
      [insertId]
    );

    context.res = {
      status: 201,
      headers: { "Content-Type": "application/json" },
      body: responses.success(rows[0]),
    };
  } catch (err) {
    context.log.error("Failed to create account", err);
    context.res = {
      status: 500,
      headers: { "Content-Type": "application/json" },
      body: responses.error("Failed to create account", "db_error"),
    };
  }
};
