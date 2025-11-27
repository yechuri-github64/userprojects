const pool = require("../../helpers/mysql/connection");
const checkApiKey = require("../../helpers/auth");
const responses = require("../../helpers/responses");

module.exports = async function (context, req) {
  context.log("HTTP trigger - Create Task");
  if (!checkApiKey(req)) {
    context.log("Unauthorized request to create task");
    context.res = {
      status: 401,
      headers: { "Content-Type": "application/json" },
      body: responses.error("Unauthorized", "unauthorized"),
    };
    return;
  }

  try {
    const body = req.body || {};
    const title = body.title && String(body.title).trim();

    if (!title) {
      context.res = {
        status: 400,
        headers: { "Content-Type": "application/json" },
        body: responses.error("title is required", "invalid_input"),
      };
      return;
    }

    const [result] = await pool.execute(
      "INSERT INTO tasks (title, completed) VALUES (?, ?)",
      [title, false]
    );
    const id = result.insertId;
    const [rows] = await pool.execute(
      "SELECT id, title, completed, created_at FROM tasks WHERE id = ?",
      [id]
    );

    context.res = {
      status: 201,
      headers: { "Content-Type": "application/json" },
      body: responses.success(rows[0]),
    };
  } catch (err) {
    context.log.error("Failed to create task", err);
    context.res = {
      status: 500,
      headers: { "Content-Type": "application/json" },
      body: responses.error("Failed to create task", "db_error"),
    };
  }
};
