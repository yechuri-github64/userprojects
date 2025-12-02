const jsforce = require("jsforce");

module.exports.getConnection = async function () {
  try {
    const loginUrl = process.env.SF_LOGIN_URL || "https://login.salesforce.com";
    const username = process.env.SF_USERNAME;
    const password = process.env.SF_PASSWORD || "";
    const token = process.env.SF_TOKEN || "";

    if (!username || (!password && !token)) {
      throw new Error(
        "Missing Salesforce credentials in environment variables"
      );
    }

    const conn = new jsforce.Connection({ loginUrl });
    await conn.login(username, password + token);
    return conn;
  } catch (err) {
    console.log("Error establishing Salesforce connection", err);
    throw err;
  }
};
