module.exports = async function (context, req) {
  try {
    const action = context.bindingData && context.bindingData.action;
    const payload = req.body;
    const db = require("../../helpers/mysql/connection");

    if (action === "batch") {
      if (!Array.isArray(payload)) {
        context.res = {
          status: 400,
          body: {
            success: false,
            error: "Expected an array of accounts for batch create",
          },
        };
        return;
      }
      const created = await db.batchCreate(payload);
      context.res = {
        status: 201,
        body: { success: true, data: created },
      };
      return;
    }

    // Single create
    if (!payload || !payload.name || !payload.email) {
      context.res = {
        status: 400,
        body: { success: false, error: "name and email are required" },
      };
      return;
    }

    const created = await db.createAccount(payload);
    context.res = {
      status: 201,
      body: { success: true, data: created },
    };
  } catch (err) {
    context.log.error(err);
    context.res = {
      status: 500,
      body: { success: false, error: err.message },
    };
  }
};
