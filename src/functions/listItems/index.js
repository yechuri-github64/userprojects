module.exports = async (req, context) => {
  try {
    const { getConnection } = require("../../helpers/salesforce/connection");
    const conn = await getConnection();
    const records = await conn
      .sobject("Account")
      .find({}, { Id: 1, Name: 1, Industry: 1, Type: 1, Phone: 1 })
      .limit(200)
      .execute();
    return { status: 200, jsonBody: { success: true, data: records } };
  } catch (err) {
    console.log(err);
    return {
      status: 500,
      jsonBody: {
        success: false,
        error: {
          message: "Server error during list",
          details: err && err.message ? err.message : err,
        },
      },
    };
  }
};
