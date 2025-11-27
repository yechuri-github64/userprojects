const pool = require("../../helpers/mysql/connection");
const checkApiKey = require("../../helpers/auth");
const responses = require("../../helpers/responses");

module.exports = async function (context, req) {
  context.log("HTTP trigger - List Accounts");
  if (!checkApiKey(req)) {
    context.log("Unauthorized request to list accounts");
    context.res = {
      status: 401,
      headers: { "Content-Type": "application/json" },
      body: responses.error("Unauthorized", "unauthorized"),
    };
    return;
  }

  try {
    const [rows] = await pool.execute(
      "SELECT id, name, email, created_at FROM accounts ORDER BY id DESC"
    );
    context.res = {
      status: 200,
      headers: { "Content-Type": "application/json" },
      body: responses.success(rows),
    };
  } catch (err) {
    context.log.error("Failed to list accounts", err);
    context.res = {
      status: 500,
      headers: { "Content-Type": "application/json" },
      body: responses.error("Failed to list accounts", "db_error"),
    };
  }
};
