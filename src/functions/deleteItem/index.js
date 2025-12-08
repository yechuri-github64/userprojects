const { getConnection } = require("../../helpers/salesforce/connection");

module.exports = async (req, context) => {
  try {
    const id = req.params && req.params.id;
    if (!id) {
      return {
        status: 400,
        jsonBody: {
          success: false,
          error: { message: "Missing id parameter" },
        },
      };
    }
    const conn = await getConnection();
    const result = await conn.sobject("Account").destroy(id);
    if (!result || result.success === false) {
      return {
        status: 500,
        jsonBody: {
          success: false,
          error: { message: "Failed to delete account", detail: result },
        },
      };
    }
    return { status: 200, jsonBody: { success: true, data: { id } } };
  } catch (err) {
    console.log("deleteItem error", err && err.message ? err.message : err);
    return {
      status: 500,
      jsonBody: {
        success: false,
        error: { message: err.message || String(err) },
      },
    };
  }
};
