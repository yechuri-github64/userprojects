try {
  const model = require('../models/accountsModel');

  async function getAccount(req, res) {
    try {
      const id = parseInt(req.params.id, 10);
      if (isNaN(id)) {
        return res.status(400).json({ error: { message: 'Invalid id' } });
      }
      const account = await model.getById(id);
      if (!account) {
        return res.status(404).json({ error: { message: 'Account not found' } });
      }
      return res.json(account);
    } catch (err) {
      console.error("Failed", err);
      return res.status(500).json({ error: { message: 'Internal Server Error' } });
    }
  }

  async function createAccounts(req, res) {
    try {
      // Accept either { accounts: [...] } or raw array in body
      const accountsInput = Array.isArray(req.body) ? req.body : req.body.accounts;
      if (!Array.isArray(accountsInput)) {
        return res.status(400).json({ error: { message: 'Request body must contain an array of accounts' } });
      }
      const created = await model.createMultiple(accountsInput);
      return res.status(201).json({ created });
    } catch (err) {
      console.error("Failed", err);
      return res.status(500).json({ error: { message: 'Internal Server Error' } });
    }
  }

  async function updateAccount(req, res) {
    try {
      const id = parseInt(req.params.id, 10);
      if (isNaN(id)) {
        return res.status(400).json({ error: { message: 'Invalid id' } });
      }
      const updates = {};
      if (req.body.name !== undefined) updates.name = req.body.name;
      if (req.body.email !== undefined) updates.email = req.body.email;
      if (req.body.address !== undefined) updates.address = req.body.address;

      const updated = await model.updateById(id, updates);
      if (!updated) {
        return res.status(404).json({ error: { message: 'Account not found' } });
      }
      return res.json(updated);
    } catch (err) {
      console.error("Failed", err);
      return res.status(500).json({ error: { message: 'Internal Server Error' } });
    }
  }

  async function deleteAccount(req, res) {
    try {
      const id = parseInt(req.params.id, 10);
      if (isNaN(id)) {
        return res.status(400).json({ error: { message: 'Invalid id' } });
      }
      const deleted = await model.deleteById(id);
      return res.json({ deleted, id });
    } catch (err) {
      console.error("Failed", err);
      return res.status(500).json({ error: { message: 'Internal Server Error' } });
    }
  }

  console.log("Connected");
  module.exports = {
    getAccount,
    createAccounts,
    updateAccount,
    deleteAccount
  };
} catch (err) {
  console.error("Failed", err);
  module.exports = {
    getAccount: async (req, res) => res.status(500).json({ error: { message: 'Controller initialization failed' } }),
    createAccounts: async (req, res) => res.status(500).json({ error: { message: 'Controller initialization failed' } }),
    updateAccount: async (req, res) => res.status(500).json({ error: { message: 'Controller initialization failed' } }),
    deleteAccount: async (req, res) => res.status(500).json({ error: { message: 'Controller initialization failed' } })
  };
}
