const { getConnection } = require("../../helpers/salesforce/connection");

module.exports = async (req, context) => {
  try {
    const body = await req.json();
    const items = Array.isArray(body)
      ? body
      : Array.isArray(body && body.accounts)
        ? body.accounts
        : null;
    if (!items || !Array.isArray(items) || items.length === 0) {
      return {
        status: 400,
        jsonBody: {
          success: false,
          error: {
            message: "Request body must be an array of account objects",
          },
        },
      };
    }
    const payload = items.map((i) => ({
      Name: i.name,
      Email__c: i.email || null,
      Address__c: i.address || null,
    }));
    const conn = await getConnection();
    // Use bulk create or array create
    const results = await conn
      .sobject("Account")
      .create(payload, { allOrNone: false });
    // results can be array of result objects
    const mapped = results.map((r) => ({
      id: r.id || null,
      success: r.success === true,
      errors: r.errors || r,
    }));
    return { status: 201, jsonBody: { success: true, data: mapped } };
  } catch (err) {
    console.log("batchCreate error", err && err.message ? err.message : err);
    return {
      status: 500,
      jsonBody: {
        success: false,
        error: { message: err.message || String(err) },
      },
    };
  }
};
