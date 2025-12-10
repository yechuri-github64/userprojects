const { pool } = require("../../helpers/mysql/connection");

module.exports = async function (context, req) {
  context.log("Post (batch create) accounts function invoked");
  let conn;
  try {
    const items = req && req.body ? req.body : null;
    if (!Array.isArray(items) || items.length === 0) {
      context.res = {
        status: 400,
        body: {
          success: false,
          error: {
            message:
              "Request body must be a non-empty array of account objects",
          },
        },
      };
      return;
    }

    conn = await pool.getConnection();
    await conn.beginTransaction();

    const created = [];
    for (const item of items) {
      const name = item && item.name ? item.name : null;
      const email = item && item.email ? item.email : null;
      const address = item && item.address ? item.address : null;

      if (!name || !email) {
        throw new Error("Each account must include at least name and email");
      }

      const [result] = await conn.execute(
        "INSERT INTO accounts (name, email, address) VALUES (?, ?, ?)",
        [name, email, address]
      );
      created.push({ id: result.insertId, name, email, address });
    }

    await conn.commit();
    context.res = {
      status: 201,
      body: created,
    };
  } catch (err) {
    if (conn) {
      try {
        await conn.rollback();
      } catch (rbErr) {
        context.log.error("Rollback error", rbErr);
      }
      try {
        conn.release();
      } catch (relErr) {
        context.log.error("Release error", relErr);
      }
    }
    context.log.error("Error creating accounts", err);
    context.res = {
      status: 500,
      body: {
        success: false,
        error: { message: "Failed to create accounts", details: err.message },
      },
    };
  } finally {
    if (conn) {
      try {
        conn.release();
      } catch (e) {
        /* ignore */
      }
    }
  }
};
