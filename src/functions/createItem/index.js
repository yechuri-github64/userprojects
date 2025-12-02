module.exports = async (req, context) => {
  try {
    const body = await req.json();
    if (!body || typeof body !== "object" || Array.isArray(body)) {
      return {
        status: 400,
        jsonBody: {
          success: false,
          error: {
            message:
              "Request body must be a JSON object representing an Account",
          },
        },
      };
    }
    const { getConnection } = require("../../helpers/salesforce/connection");
    const conn = await getConnection();
    const result = await conn.sobject("Account").create(body);
    if (result && result.success) {
      return { status: 201, jsonBody: { success: true, data: result } };
    }
    console.log("Salesforce create failed", result);
    return {
      status: 500,
      jsonBody: {
        success: false,
        error: { message: "Failed to create Account", details: result },
      },
    };
  } catch (err) {
    console.log(err);
    return {
      status: 500,
      jsonBody: {
        success: false,
        error: {
          message: "Server error during create",
          details: err && err.message ? err.message : err,
        },
      },
    };
  }
};
