module.exports = async (req, context) => {
  try {
    const body = await req.json();
    if (!Array.isArray(body)) {
      return {
        status: 400,
        jsonBody: {
          success: false,
          error: {
            message:
              "Request body must be a JSON array of Account objects for batch creation",
          },
        },
      };
    }
    const { getConnection } = require("../../helpers/salesforce/connection");
    const conn = await getConnection();
    const results = await conn
      .sobject("Account")
      .create(body, { allOrNone: false });
    const normalized = Array.isArray(results) ? results : [results];
    return { status: 201, jsonBody: { success: true, data: normalized } };
  } catch (err) {
    console.log(err);
    return {
      status: 500,
      jsonBody: {
        success: false,
        error: {
          message: "Server error during batch create",
          details: err && err.message ? err.message : err,
        },
      },
    };
  }
};
