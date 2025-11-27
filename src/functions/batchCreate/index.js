const { getConnection } = require("../../helpers/salesforce/connection");

module.exports = async (req, context) => {
  try {
    const body = await req.json();
    if (!Array.isArray(body)) {
      return {
        status: 400,
        jsonBody: {
          success: false,
          error: { message: "Expected an array of accounts" },
        },
      };
    }

    const records = body.map((item) => ({
      Name: item.name,
      Email__c: item.email || null,
      Address__c: item.address || null,
    }));

    const conn = await getConnection();
    const results = await conn.sobject("Account").create(records);

    // Normalize results to array
    const normalized = Array.isArray(results) ? results : [results];
    const response = normalized.map((r, idx) => ({
      success: !!r.success || !!r.id,
      id: r.id || null,
      errors: r.errors || null,
      input: body[idx],
    }));

    return { status: 201, jsonBody: { success: true, data: response } };
  } catch (err) {
    console.log(err);
    return {
      status: 500,
      jsonBody: { success: false, error: { message: err.message } },
    };
  }
};
