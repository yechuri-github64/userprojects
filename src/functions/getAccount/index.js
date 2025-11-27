const pool = require("../../helpers/mysql/connection");
const checkApiKey = require("../../helpers/auth");
const responses = require("../../helpers/responses");

module.exports = async function (context, req) {
  context.log("HTTP trigger - Get Account");
  if (!checkApiKey(req)) {
    context.log("Unauthorized request to get account");
    context.res = {
      status: 401,
      headers: { "Content-Type": "application/json" },
      body: responses.error("Unauthorized", "unauthorized"),
    };
    return;
  }

  try {
    const id = req.params && req.params.id;
    if (!id) {
      context.res = {
        status: 400,
        headers: { "Content-Type": "application/json" },
        body: responses.error("id is required", "invalid_input"),
      };
      return;
    }

    const [rows] = await pool.execute(
      "SELECT id, name, email, created_at FROM accounts WHERE id = ?",
      [id]
    );
    if (!rows || rows.length === 0) {
      context.res = {
        status: 404,
        headers: { "Content-Type": "application/json" },
        body: responses.error("Account not found", "not_found"),
      };
      return;
    }

    context.res = {
      status: 200,
      headers: { "Content-Type": "application/json" },
      body: responses.success(rows[0]),
    };
  } catch (err) {
    context.log.error("Failed to get account", err);
    context.res = {
      status: 500,
      headers: { "Content-Type": "application/json" },
      body: responses.error("Failed to get account", "db_error"),
    };
  }
};
