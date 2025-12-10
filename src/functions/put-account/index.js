const { execute, pool } = require("../../helpers/mysql/connection");

module.exports = async function (context, req) {
  context.log("Put (update) account function invoked");
  const id =
    context.bindingData && context.bindingData.id
      ? context.bindingData.id
      : req && req.query && req.query.id
        ? req.query.id
        : null;
  if (!id) {
    context.res = {
      status: 400,
      body: {
        success: false,
        error: { message: "Account id is required in the route" },
      },
    };
    return;
  }

  try {
    const body = req && req.body ? req.body : {};
    // Allow partial updates, but only one account at a time (single id)
    const fields = [];
    const params = [];

    if (body.name !== undefined) {
      fields.push("name = ?");
      params.push(body.name);
    }
    if (body.email !== undefined) {
      fields.push("email = ?");
      params.push(body.email);
    }
    if (body.address !== undefined) {
      fields.push("address = ?");
      params.push(body.address);
    }

    if (fields.length === 0) {
      context.res = {
        status: 400,
        body: {
          success: false,
          error: {
            message:
              "No updatable fields provided. Provide at least one of: name, email, address",
          },
        },
      };
      return;
    }

    params.push(id);
    const sql = `UPDATE accounts SET ${fields.join(", ")} WHERE id = ?`;
    const result = await pool.execute(sql, params);
    // result may differ depending on mysql2; use a follow-up select
    const rows = await execute(
      "SELECT id, name, email, address FROM accounts WHERE id = ?",
      [id]
    );
    if (!rows || rows.length === 0) {
      context.res = {
        status: 404,
        body: {
          success: false,
          error: { message: "Account not found after update", id },
        },
      };
      return;
    }

    context.res = {
      status: 200,
      body: rows[0],
    };
  } catch (err) {
    context.log.error("Error updating account", err);
    context.res = {
      status: 500,
      body: {
        success: false,
        error: { message: "Failed to update account", details: err.message },
      },
    };
  }
};
