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
    const body = await req.json();
    if (!body || typeof body !== "object") {
      return {
        status: 400,
        jsonBody: {
          success: false,
          error: {
            message: "Request body must be a JSON object with fields to update",
          },
        },
      };
    }
    const { getConnection } = require("../../helpers/salesforce/connection");
    const conn = await getConnection();
    const updateObj = Object.assign({ Id: id }, body);
    const result = await conn.sobject("Account").update(updateObj);
    if (result && result.success) {
      return { status: 200, jsonBody: { success: true, data: result } };
    }
    console.log("Salesforce update failed", result);
    return {
      status: 500,
      jsonBody: {
        success: false,
        error: { message: "Failed to update Account", details: result },
      },
    };
  } catch (err) {
    console.log(err);
    return {
      status: 500,
      jsonBody: {
        success: false,
        error: {
          message: "Server error during update",
          details: err && err.message ? err.message : err,
        },
      },
    };
  }
};
