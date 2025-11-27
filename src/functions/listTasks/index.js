const pool = require("../../helpers/mysql/connection");
const checkApiKey = require("../../helpers/auth");
const responses = require("../../helpers/responses");

module.exports = async function (context, req) {
  context.log("HTTP trigger - List Tasks");
  if (!checkApiKey(req)) {
    context.log("Unauthorized request to list tasks");
    context.res = {
      status: 401,
      headers: { "Content-Type": "application/json" },
      body: responses.error("Unauthorized", "unauthorized"),
    };
    return;
  }

  try {
    const [rows] = await pool.execute(
      "SELECT id, title, completed, created_at FROM tasks ORDER BY created_at DESC"
    );
    context.res = {
      status: 200,
      headers: { "Content-Type": "application/json" },
      body: responses.success(rows),
    };
  } catch (err) {
    context.log.error("Failed to list tasks", err);
    context.res = {
      status: 500,
      headers: { "Content-Type": "application/json" },
      body: responses.error("Failed to list tasks", "db_error"),
    };
  }
};
