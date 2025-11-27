module.exports = async function (context, req) {
  try {
    const id = context.bindingData && context.bindingData.id;
    if (!id) {
      context.res = {
        status: 400,
        body: { success: false, error: "id is required" },
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
    await db.deleteAccount(id);
    context.res = { status: 200, body: { success: true, data: null } };
  } catch (err) {
    context.log.error(err);
    context.res = { status: 500, body: { success: false, error: err.message } };
  }
};
