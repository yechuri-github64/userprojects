const axios = require('axios');
const { delay } = require('./utils');

class SalesforceService {
  constructor() {
    this.clientId = process.env.SALESFORCE_CLIENT_ID || '';
    this.clientSecret = process.env.SALESFORCE_CLIENT_SECRET || '';
    this.username = process.env.SALESFORCE_USERNAME || '';
    this.password = (process.env.SALESFORCE_PASSWORD || '') + (process.env.SALESFORCE_SECURITY_TOKEN || '');
    this.loginUrl = process.env.SALESFORCE_LOGIN_URL || 'https://login.salesforce.com';
    this.instanceUrl = process.env.SALESFORCE_INSTANCE_URL || null;
    this.apiVersion = process.env.SALESFORCE_API_VERSION || '59.0';
    this.timeout = parseInt(process.env.SALESFORCE_TIMEOUT_SECONDS || '120', 10) * 1000;
    this.enableLogging = (process.env.SALESFORCE_ENABLE_LOGGING || 'true') === 'true';
    this.maxRetries = parseInt(process.env.SALESFORCE_MAX_RETRIES || '3', 10);
    this.retryDelay = parseInt(process.env.SALESFORCE_RETRY_DELAY || '3', 10) * 1000;

    this.token = null;
    this.tokenExpiry = 0;
  }

  async authenticate() {
    const now = Date.now();
    if (this.token && now < this.tokenExpiry) return this.token;

    const url = `${this.loginUrl.replace(/\/+$/, '')}/services/oauth2/token`;
    const params = new URLSearchParams();
    params.append('grant_type', 'password');
    params.append('client_id', this.clientId);
    params.append('client_secret', this.clientSecret);
    params.append('username', this.username);
    params.append('password', this.password);

    try {
      if (this.enableLogging) console.log('Authenticating to Salesforce');
      const res = await axios.post(url, params.toString(), {
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        timeout: this.timeout
      });
      const data = res.data;
      this.token = data.access_token;
      this.instanceUrl = data.instance_url || this.instanceUrl;
      const expiresIn = parseInt(data.expires_in || process.env.SALESFORCE_TIMEOUT_SECONDS || '120', 10);
      this.tokenExpiry = Date.now() + (expiresIn - 10) * 1000;
      if (this.enableLogging) console.log('Salesforce authentication successful');
      return this.token;
    } catch (err) {
      console.error('Salesforce authentication failed:', err && err.response ? err.response.data : err.message || err);
      const error = new Error('Failed to authenticate with Salesforce');
      error.statusCode = err.response && err.response.status ? err.response.status : 502;
      throw error;
    }
  }

  async request(method, path, data = null, headers = {}) {
    const token = await this.authenticate();
    const base = (this.instanceUrl || '').replace(/\/+$/, '');
    if (!base) {
      const e = new Error('Salesforce instance URL is not configured');
      e.statusCode = 500;
      throw e;
    }
    const url = `${base}/services/data/v${this.apiVersion}${path}`;
    let attempt = 0;
    while (true) {
      try {
        const res = await axios({
          method,
          url,
          data,
          headers: Object.assign({ Authorization: `Bearer ${token}` }, headers),
          timeout: this.timeout
        });
        return res;
      } catch (err) {
        attempt++;
        const status = err.response && err.response.status ? err.response.status : null;
        const shouldRetry = attempt <= this.maxRetries && (status === null || status >= 500);
        console.error(`Request error (attempt ${attempt}):`, err && err.response ? err.response.data : err.message || err);
        if (!shouldRetry) {
          const error = new Error(err.response && err.response.data ? JSON.stringify(err.response.data) : (err.message || 'Salesforce request failed'));
          error.statusCode = status || 502;
          throw error;
        }
        await delay(this.retryDelay);
      }
    }
  }

  async createAccount(account) {
    const res = await this.request('post', '/sobjects/Account/', account, { 'Content-Type': 'application/json' });
    if (res && res.data && res.data.id) {
      return this.getAccount(res.data.id);
    }
    const error = new Error('Failed to create account');
    error.statusCode = res && res.status ? res.status : 502;
    throw error;
  }

  async getAccount(id) {
    const res = await this.request('get', `/sobjects/Account/${encodeURIComponent(id)}`);
    return res.data;
  }

  async updateAccount(id, account) {
    await this.request('patch', `/sobjects/Account/${encodeURIComponent(id)}`, account, { 'Content-Type': 'application/json' });
    return true;
  }

  async deleteAccount(id) {
    await this.request('delete', `/sobjects/Account/${encodeURIComponent(id)}`);
    return true;
  }
}

module.exports = SalesforceService;
