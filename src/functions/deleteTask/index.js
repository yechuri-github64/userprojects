const pool = require("../../helpers/mysql/connection");
const checkApiKey = require("../../helpers/auth");
const responses = require("../../helpers/responses");

module.exports = async function (context, req) {
  context.log("HTTP trigger - Delete Task");
  if (!checkApiKey(req)) {
    context.log("Unauthorized request to delete task");
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

    const [result] = await pool.execute("DELETE FROM tasks WHERE id = ?", [id]);
    if (result.affectedRows === 0) {
      context.res = {
        status: 404,
        headers: { "Content-Type": "application/json" },
        body: responses.error("Task not found", "not_found"),
      };
      return;
    }

    context.res = {
      status: 200,
      headers: { "Content-Type": "application/json" },
      body: responses.success({ id }),
    };
  } catch (err) {
    context.log.error("Failed to delete task", err);
    context.res = {
      status: 500,
      headers: { "Content-Type": "application/json" },
      body: responses.error("Failed to delete task", "db_error"),
    };
  }
};
