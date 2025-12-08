const { getConnection } = require("../../helpers/salesforce/connection");

module.exports = async (req, context) => {
  try {
    const conn = await getConnection();
    const body = await req.json();
    if (!body || !body.name) {
      return {
        status: 400,
        jsonBody: {
          success: false,
          error: { message: "Missing required field: name" },
        },
      };
    }
    const payload = {
      Name: body.name,
      Email__c: body.email || null,
      Address__c: body.address || null,
    };
    const result = await conn.sobject("Account").create(payload);
    if (!result || !result.id) {
      return {
        status: 500,
        jsonBody: {
          success: false,
          error: { message: "Failed to create account", detail: result },
        },
      };
    }
    return {
      status: 201,
      jsonBody: { success: true, data: { id: result.id } },
    };
  } catch (err) {
    console.log("createItem error", err && err.message ? err.message : err);
    return {
      status: 500,
      jsonBody: {
        success: false,
        error: { message: err.message || String(err) },
      },
    };
  }
};
