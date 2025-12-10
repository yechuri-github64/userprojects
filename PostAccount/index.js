const { pool } = require("../src/helpers/mysql/connection");

module.exports = async function (context, req) {
  context.log("PostAccount invoked");
  let connection;
  try {
    const payload = req.body;
    const items = Array.isArray(payload)
      ? payload
      : payload && Array.isArray(payload.accounts)
        ? payload.accounts
        : null;
    if (!items || !Array.isArray(items) || items.length === 0) {
      context.res = {
        status: 400,
        body: { error: "Invalid input, expected array of accounts" },
      };
      return;
    }

    connection = await pool.getConnection();
    await connection.beginTransaction();

    const created = [];
    for (const item of items) {
      const name = item.name;
      const email = item.email;
      const address = item.address || null;
      if (!name || !email) {
        await connection.rollback();
        context.res = {
          status: 400,
          body: { error: "Each account must have name and email" },
        };
        return;
      }
      const [result] = await connection.execute(
        "INSERT INTO accounts (name, email, address) VALUES (?, ?, ?)",
        [name, email, address]
      );
      created.push({ id: result.insertId, name, email, address });
    }

    await connection.commit();
    context.res = { status: 201, body: created };
  } catch (err) {
    try {
      if (connection) {
        await connection.rollback();
      }
    } catch (rbErr) {
      context.log.error("Rollback error", rbErr);
    }
    context.log.error(err);
    context.res = {
      status: 500,
      body: { error: "Internal Server Error", details: err.message },
    };
  } finally {
    if (connection) {
      try {
        connection.release();
      } catch (releaseErr) {
        context.log.error("Connection release error", releaseErr);
      }
    }
  }
};
