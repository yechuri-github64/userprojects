const { execute } = require("../../helpers/mysql/connection");

module.exports = async function (context, req) {
  context.log("Get account function invoked");
  try {
    const id =
      context.bindingData && context.bindingData.id
        ? context.bindingData.id
        : req && req.query && req.query.id
          ? req.query.id
          : undefined;

    if (id) {
      const rows = await execute(
        "SELECT id, name, email, address FROM accounts WHERE id = ?",
        [id]
      );
      if (!rows || rows.length === 0) {
        context.log(`Account not found: ${id}`);
        context.res = {
          status: 404,
          body: {
            success: false,
            error: { message: "Account not found", id: id },
          },
        };
        return;
      }
      context.res = {
        status: 200,
        body: rows[0],
      };
      return;
    }

    const rows = await execute(
      "SELECT id, name, email, address FROM accounts",
      []
    );
    context.res = {
      status: 200,
      body: rows,
    };
  } catch (err) {
    context.log.error("Error retrieving accounts", err);
    context.res = {
      status: 500,
      body: {
        success: false,
        error: { message: "Internal server error", details: err.message },
      },
    };
  }
};
