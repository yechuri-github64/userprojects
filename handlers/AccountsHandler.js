require('dotenv').config();
const AccountsService = require('../services/AccountsService');
const response = require('../utils/response');
module.exports = {
  get: async (event) => {
    try {
      const id = event.pathParameters && event.pathParameters.id;
      if (id) {
        const account = await AccountsService.getById(id);
        if (!account) return response.format(404, { message: 'Account not found' });
        return response.format(200, account);
      }
      const accounts = await AccountsService.getAll();
      return response.format(200, accounts);
    } catch (err) {
      console.log('get handler error', err);
      return response.format(500, { error: 'Failed to get accounts' });
    }
  },
  create: async (event) => {
    try {
      const payload = typeof event.body === 'string' ? JSON.parse(event.body) : event.body;
      if (!Array.isArray(payload)) return response.format(400, { error: 'Request body must be a JSON array of accounts' });
      const created = await AccountsService.createMultiple(payload);
      return response.format(201, { inserted: created.insertId || null, affectedRows: created.affectedRows || 0 });
    } catch (err) {
      console.log('create handler error', err);
      return response.format(500, { error: 'Failed to create accounts' });
    }
  },
  update: async (event) => {
    try {
      const id = event.pathParameters && event.pathParameters.id;
      if (!id) return response.format(400, { error: 'Missing account id in path' });
      const payload = typeof event.body === 'string' ? JSON.parse(event.body) : event.body;
      if (!payload || typeof payload !== 'object') return response.format(400, { error: 'Request body must be a JSON object with fields to update' });
      const result = await AccountsService.updateOne(id, payload);
      if (result.affectedRows === 0) return response.format(404, { message: 'Account not found or no changes made' });
      return response.format(200, { affectedRows: result.affectedRows });
    } catch (err) {
      console.log('update handler error', err);
      return response.format(500, { error: 'Failed to update account' });
    }
  },
  remove: async (event) => {
    try {
      const id = event.pathParameters && event.pathParameters.id;
      if (!id) return response.format(400, { error: 'Missing account id in path' });
      const result = await AccountsService.deleteOne(id);
      if (result.affectedRows === 0) return response.format(404, { message: 'Account not found' });
      return response.format(200, { affectedRows: result.affectedRows });
    } catch (err) {
      console.log('remove handler error', err);
      return response.format(500, { error: 'Failed to delete account' });
    }
  }
};