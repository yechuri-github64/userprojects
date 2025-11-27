module.exports = async function (context, req) {
  try {
    const db = require("../../helpers/mysql/connection");
    const items = await db.listAccounts();
    context.res = {
      status: 200,
      body: { success: true, data: items },
    };
  } catch (err) {
    context.log.error(err);
    context.res = {
      status: 500,
      body: { success: false, error: err.message },
    };
  }
};
