require('dotenv').config();
const jsforce = require('jsforce');
const retry = require('../utils/retry');

class SalesforceService {
  constructor() {
    this.conn = null;
    this.loginUrl = process.env.SALESFORCE_LOGIN_URL || 'https://login.salesforce.com';
    this.instanceUrl = process.env.SALESFORCE_INSTANCE_URL || undefined;
    this.clientId = process.env.SALESFORCE_CLIENT_ID;
    this.clientSecret = process.env.SALESFORCE_CLIENT_SECRET;
    this.username = process.env.SALESFORCE_USERNAME;
    this.password = process.env.SALESFORCE_PASSWORD;
    this.securityToken = process.env.SALESFORCE_SECURITY_TOKEN || '';
    this.apiVersion = process.env.SALESFORCE_API_VERSION || '59.0';
    this.maxRetries = parseInt(process.env.SALESFORCE_MAX_RETRIES || '3', 10);
    this.retryDelay = parseInt(process.env.SALESFORCE_RETRY_DELAY || '3', 10) * 1000;
    this.enableLogging = String(process.env.SALESFORCE_ENABLE_LOGGING || 'false') === 'true';
  }

  async ensureConnected() {
    if (this.conn && this.conn.accessToken) return;

    const attemptLogin = async () => {
      console.log('SalesforceService: creating connection', { loginUrl: this.loginUrl, apiVersion: this.apiVersion });
      const connOptions = { loginUrl: this.loginUrl, version: this.apiVersion };
      this.conn = new jsforce.Connection(connOptions);
      const pwd = `${this.password || ''}${this.securityToken || ''}`;
      if (!this.username || !this.password) {
        const err = new Error('Salesforce credentials are not set in environment variables');
        err.statusCode = 500;
        throw err;
      }
      console.log('SalesforceService: logging in as', this.username.replace(/(.{2}).+/, '$1***'));
      const loginResult = await this.conn.login(this.username, pwd);
      if (this.enableLogging) console.log('SalesforceService: loginResult', loginResult);
      if (this.instanceUrl) this.conn.instanceUrl = this.instanceUrl;
      return loginResult;
    };

    await retry(attemptLogin, this.maxRetries, this.retryDelay);
  }

  async updateOrder(orderId, payload) {
    if (!orderId) {
      const err = new Error('orderId is required');
      err.statusCode = 400;
      throw err;
    }
    await this.ensureConnected();
    try {
      const toUpdate = Object.assign({ Id: orderId }, payload || {});
      if (this.enableLogging) console.log('SalesforceService: updating Order sObject', toUpdate);
      const res = await this.conn.sobject('Order').update(toUpdate);
      if (!res || !res.success) {
        const error = new Error('Failed to update Order');
        error.statusCode = 502;
        error.details = res;
        throw error;
      }

      // Retrieve full record after update
      const record = await this.retrieveOrder(orderId);
      return record;
    } catch (err) {
      console.error('SalesforceService.updateOrder error', err && err.message ? err.message : err);
      throw err;
    }
  }

  async retrieveOrder(orderId) {
    await this.ensureConnected();
    try {
      const record = await this.conn.sobject('Order').retrieve(orderId);
      return record;
    } catch (err) {
      console.error('SalesforceService.retrieveOrder error', err);
      throw err;
    }
  }
}

module.exports = SalesforceService;
