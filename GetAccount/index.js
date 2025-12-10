const { execute } = require("../src/helpers/mysql/connection");

module.exports = async function (context, req) {
  context.log("GetAccount invoked");
  try {
    const id = req.params && req.params.id;
    if (id) {
      const rows = await execute(
        "SELECT id, name, email, address FROM accounts WHERE id = ?",
        [id]
      );
      if (!rows || rows.length === 0) {
        context.res = { status: 404, body: { error: "Account not found" } };
        return;
      }
      context.res = { status: 200, body: rows[0] };
      return;
    }

    const rows = await execute(
      "SELECT id, name, email, address FROM accounts",
      []
    );
    context.res = { status: 200, body: rows };
  } catch (err) {
    context.log.error(err);
    context.res = {
      status: 500,
      body: { error: "Internal Server Error", details: err.message },
    };
  }
};
