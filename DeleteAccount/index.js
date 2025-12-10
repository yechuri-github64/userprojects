const { execute } = require("../src/helpers/mysql/connection");

module.exports = async function (context, req) {
  context.log("DeleteAccount invoked");
  try {
    const id = req.params && req.params.id;
    if (!id) {
      context.res = { status: 400, body: { error: "Missing id parameter" } };
      return;
    }

    const result = await execute("DELETE FROM accounts WHERE id = ?", [id]);
    if (!result || result.affectedRows === 0) {
      context.res = { status: 404, body: { error: "Account not found" } };
      return;
    }

    context.res = {
      status: 200,
      body: { message: "Account deleted", id: Number(id) },
    };
  } catch (err) {
    context.log.error(err);
    context.res = {
      status: 500,
      body: { error: "Internal Server Error", details: err.message },
    };
  }
};
