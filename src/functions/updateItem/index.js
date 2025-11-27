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
    if (
      !body ||
      (body.name === undefined &&
        body.email === undefined &&
        body.address === undefined)
    ) {
      return {
        status: 400,
        jsonBody: {
          success: false,
          error: { message: "No fields provided to update" },
        },
      };
    }
    const fields = { Id: id };
    if (body.name !== undefined) fields.Name = body.name;
    if (body.email !== undefined) fields.Email__c = body.email;
    if (body.address !== undefined) fields.Address__c = body.address;

    const conn = await getConnection();
    const result = await conn.sobject("Account").update(fields);

    if (!result || result.success === false) {
      console.log("Update failed", result);
      return {
        status: 500,
        jsonBody: {
          success: false,
          error: { message: "Failed to update account", details: result },
        },
      };
    }

    return { status: 200, jsonBody: { success: true, data: { id } } };
  } catch (err) {
    console.log(err);
    return {
      status: 500,
      jsonBody: { success: false, error: { message: err.message } },
    };
  }
};
