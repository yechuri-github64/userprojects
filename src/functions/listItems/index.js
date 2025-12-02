module.exports = async (req, context) => {
  try {
    const { getConnection } = require("../../helpers/salesforce/connection");
    const conn = await getConnection();
    let records = [];
    // Prefer query if available (common on full jsforce.Connection)
    if (conn && typeof conn.query === 'function') {
      const soql = 'SELECT Id, Name, Industry, Type, Phone FROM Account LIMIT 200';
      const res = await conn.query(soql);
      records = (res && res.records) ? res.records : [];
    } else {
      const sObj = conn && typeof conn.sobject === 'function' ? conn.sobject('Account') : null;
      if (sObj && typeof sObj.find === 'function') {
        records = await sObj.find({}, { Id: 1, Name: 1, Industry: 1, Type: 1, Phone: 1 }).limit(200).execute();
      } else {
        // Neither query nor sobject().find are available on the connection — log helpful debug info
        const connShape = {};
        try {
          connShape.type = conn && conn.constructor && conn.constructor.name;
          connShape.hasQuery = !!(conn && typeof conn.query === 'function');
          connShape.hasSobject = !!(conn && typeof conn.sobject === 'function');
          connShape.keys = conn && typeof conn === 'object' ? Object.keys(conn).slice(0, 20) : [];
        } catch (e) {
          // ignore
        }
        console.error('Salesforce connection missing query and sobject.find:', connShape);
        throw new Error('Salesforce connection does not support query or sobject().find');
      }
    }
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
