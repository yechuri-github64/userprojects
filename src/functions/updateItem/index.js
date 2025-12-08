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
    const body = await req.json();
    if (!body) {
      return {
        status: 400,
        jsonBody: {
          success: false,
          error: { message: "Missing body with fields to update" },
        },
      };
    }
    const payload = { Id: id };
    if (body.name !== undefined) payload.Name = body.name;
    if (body.email !== undefined) payload.Email__c = body.email;
    if (body.address !== undefined) payload.Address__c = body.address;

    const conn = await getConnection();
    const result = await conn.sobject("Account").update(payload);
    if (!result || result.success === false) {
      return {
        status: 500,
        jsonBody: {
          success: false,
          error: { message: "Failed to update account", detail: result },
        },
      };
    }
    return { status: 200, jsonBody: { success: true, data: { id } } };
  } catch (err) {
    console.log("updateItem error", err && err.message ? err.message : err);
    return {
      status: 500,
      jsonBody: {
        success: false,
        error: { message: err.message || String(err) },
      },
    };
  }
};
