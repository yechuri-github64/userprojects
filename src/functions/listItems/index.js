const { getConnection } = require("../../helpers/salesforce/connection");

module.exports = async (req, context) => {
  try {
    const conn = await getConnection();
    const query =
      "SELECT Id, Name, Email__c, Address__c FROM Account LIMIT 200";
    const result = await conn.query(query);
    const records =
      result && result.records
        ? result.records.map((r) => ({
            id: r.Id,
            name: r.Name,
            email: r.Email__c,
            address: r.Address__c,
          }))
        : [];
    return { status: 200, jsonBody: { success: true, data: records } };
  } catch (err) {
    console.log(err);
    return {
      status: 500,
      jsonBody: { success: false, error: { message: err.message } },
    };
  }
};
