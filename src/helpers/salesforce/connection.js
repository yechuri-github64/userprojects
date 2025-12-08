const jsforce = require("jsforce");

let cachedConn = null;

async function createAndLogin() {
  const loginUrl = process.env.SF_LOGIN_URL || "https://login.salesforce.com";
  const username = process.env.SF_USERNAME;
  const password = process.env.SF_PASSWORD || "";
  const token = process.env.SF_TOKEN || "";
  const clientId = process.env.SF_CLIENT_ID || "";
  const clientSecret = process.env.SF_CLIENT_SECRET || "";

  if (!username || (!password && !token)) {
    throw new Error("Missing Salesforce credentials in environment variables");
  }

  const conn = new jsforce.Connection({
    loginUrl,
    oauth2: { clientId, clientSecret },
  });
  // Password concatenated with token per Salesforce login requirements
  await conn.login(username, password + (token || ""));
  return conn;
}

module.exports = {
  getConnection: async () => {
    try {
      if (cachedConn && cachedConn.accessToken) {
        return cachedConn;
      }
      cachedConn = await createAndLogin();
      return cachedConn;
    } catch (err) {
      // Ensure any failure does not keep a broken cached connection
      cachedConn = null;
      console.log(
        "Salesforce connection error",
        err && err.message ? err.message : err
      );
      throw err;
    }
  },
};
