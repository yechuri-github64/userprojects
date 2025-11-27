const { getConnection } = require("../../helpers/salesforce/connection");

module.exports = async (req, context) => {
  try {
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

    const conn = await getConnection();
    const record = {
      Name: body.name,
      Email__c: body.email || null,
      Address__c: body.address || null,
    };

    const result = await conn.sobject("Account").create(record);

    if (!result || (!result.id && !result.success)) {
      console.log("Create failed", result);
      return {
        status: 500,
        jsonBody: {
          success: false,
          error: { message: "Failed to create account", details: result },
        },
      };
    }

    return {
      status: 201,
      jsonBody: { success: true, data: { id: result.id || result } },
    };
  } catch (err) {
    console.log(err);
    return {
      status: 500,
      jsonBody: { success: false, error: { message: err.message } },
    };
  }
};
