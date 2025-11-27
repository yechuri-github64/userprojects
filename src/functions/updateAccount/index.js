const pool = require("../../helpers/mysql/connection");
const checkApiKey = require("../../helpers/auth");
const responses = require("../../helpers/responses");

module.exports = async function (context, req) {
  context.log("HTTP trigger - Update Account");
  if (!checkApiKey(req)) {
    context.log("Unauthorized request to update account");
    context.res = {
      status: 401,
      headers: { "Content-Type": "application/json" },
      body: responses.error("Unauthorized", "unauthorized"),
    };
    return;
  }

  try {
    const id = req.params && req.params.id;
    const body = req.body || {};

    if (!id) {
      context.res = {
        status: 400,
        headers: { "Content-Type": "application/json" },
        body: responses.error("id is required", "invalid_input"),
      };
      return;
    }

    const fields = [];
    const values = [];
    if (body.name !== undefined) {
      fields.push("name = ?");
      values.push(body.name);
    }
    if (body.email !== undefined) {
      fields.push("email = ?");
      values.push(body.email);
    }

    if (fields.length === 0) {
      context.res = {
        status: 400,
        headers: { "Content-Type": "application/json" },
        body: responses.error("No updatable fields provided", "invalid_input"),
      };
      return;
    }

    values.push(id);
    const sql = `UPDATE accounts SET ${fields.join(", ")} WHERE id = ?`;
    const [result] = await pool.execute(sql, values);

    if (result.affectedRows === 0) {
      context.res = {
        status: 404,
        headers: { "Content-Type": "application/json" },
        body: responses.error("Account not found", "not_found"),
      };
      return;
    }

    const [rows] = await pool.execute(
      "SELECT id, name, email, created_at FROM accounts WHERE id = ?",
      [id]
    );
    context.res = {
      status: 200,
      headers: { "Content-Type": "application/json" },
      body: responses.success(rows[0]),
    };
  } catch (err) {
    context.log.error("Failed to update account", err);
    context.res = {
      status: 500,
      headers: { "Content-Type": "application/json" },
      body: responses.error("Failed to update account", "db_error"),
    };
  }
};
