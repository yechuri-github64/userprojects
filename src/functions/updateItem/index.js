module.exports = async function (context, req) {
  try {
    const id = context.bindingData && context.bindingData.id;
    const payload = req.body;
    if (!id) {
      context.res = {
        status: 400,
        body: { success: false, error: "id is required" },
      };
      return;
    }
    if (!payload || Object.keys(payload).length === 0) {
      context.res = {
        status: 400,
        body: { success: false, error: "body is required for update" },
      };
      return;
    }
    const db = require("../../helpers/mysql/connection");
    const existing = await db.getAccount(id);
    if (!existing) {
      context.res = {
        status: 404,
        body: { success: false, error: "not found" },
      };
      return;
    }
    const updated = await db.updateAccount(id, payload);
    context.res = { status: 200, body: { success: true, data: updated } };
  } catch (err) {
    context.log.error(err);
    context.res = { status: 500, body: { success: false, error: err.message } };
  }
};
