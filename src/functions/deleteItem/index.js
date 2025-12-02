module.exports = async (req, context) => {
  try {
    const id = req.params && req.params.id;
    if (!id) {
      return {
        status: 400,
        jsonBody: {
          success: false,
          error: { message: "Missing account id in route parameters" },
        },
      };
    }
    const { getConnection } = require("../../helpers/salesforce/connection");
    const conn = await getConnection();
    const result = await conn.sobject("Account").destroy(id);
    if (result && result.success) {
      return { status: 200, jsonBody: { success: true, data: result } };
    }
    console.log("Salesforce delete failed", result);
    return {
      status: 500,
      jsonBody: {
        success: false,
        error: { message: "Failed to delete Account", details: result },
      },
    };
  } catch (err) {
    console.log(err);
    return {
      status: 500,
      jsonBody: {
        success: false,
        error: {
          message: "Server error during delete",
          details: err && err.message ? err.message : err,
        },
      },
    };
  }
};
