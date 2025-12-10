const { execute, pool } = require("../../helpers/mysql/connection");

module.exports = async function (context, req) {
  context.log("Delete account function invoked");
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
    const conn = await pool.getConnection();
    try {
      const [result] = await conn.execute("DELETE FROM accounts WHERE id = ?", [
        id,
      ]);
      conn.release();
      if (result && result.affectedRows && result.affectedRows > 0) {
        context.res = {
          status: 200,
          body: { success: true, deletedId: Number(id) },
        };
      } else {
        context.res = {
          status: 404,
          body: { success: false, error: { message: "Account not found", id } },
        };
      }
    } catch (err) {
      try {
        conn.release();
      } catch (e) {
        /* ignore */
      }
      throw err;
    }
  } catch (err) {
    context.log.error("Error deleting account", err);
    context.res = {
      status: 500,
      body: {
        success: false,
        error: { message: "Failed to delete account", details: err.message },
      },
    };
  }
};
