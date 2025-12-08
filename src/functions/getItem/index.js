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
    const record = await conn.sobject("Account").retrieve(id);
    if (!record || !record.Id) {
      return {
        status: 404,
        jsonBody: { success: false, error: { message: "Account not found" } },
      };
    }
    const data = {
      id: record.Id,
      name: record.Name,
      email: record.Email__c,
      address: record.Address__c,
    };
    return { status: 200, jsonBody: { success: true, data } };
  } catch (err) {
    console.log("getItem error", err && err.message ? err.message : err);
    return {
      status: 500,
      jsonBody: {
        success: false,
        error: { message: err.message || String(err) },
      },
    };
  }
};
