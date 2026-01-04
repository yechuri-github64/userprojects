const jsforce = require('jsforce');
const config = require('./config/salesforce');

let conn = new jsforce.Connection({ loginUrl: config.SALESFORCE_LOGIN_URL });

(async () => {
  try {
    const username = config.SALESFORCE_USERNAME;
    const password = config.SALESFORCE_PASSWORD || '';
    const token = config.SALESFORCE_TOKEN || '';

    if (!username) {
      throw new Error('Missing SALESFORCE_USERNAME in environment');
    }

    await conn.login(username, password + token);
    console.log('connections/salesforce: Connected');
  } catch (err) {
    console.error('connections/salesforce: Failed', err);
  }
})();

module.exports = conn;
