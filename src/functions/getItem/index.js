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
    const record = await conn.sobject("Account").retrieve(id);
    if (!record) {
      return {
        status: 404,
        jsonBody: { success: false, error: { message: "Account not found" } },
      };
    }
    return { status: 200, jsonBody: { success: true, data: record } };
  } catch (err) {
    console.log(err);
    return {
      status: 500,
      jsonBody: {
        success: false,
        error: {
          message: "Server error during get",
          details: err && err.message ? err.message : err,
        },
      },
    };
  }
};
