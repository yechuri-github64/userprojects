const connection = require('../../connections/mysql');

module.exports = {
  getAccount: async (req, res) => {
    try {
      const id = req.params.id;
      connection.query('SELECT id, name, email, address FROM accounts WHERE id = ?', [id], (err, results) => {
        if (err) {
          console.error('getAccount Failed', err);
          return res.status(500).json({ error: { message: 'Database error', details: err.message } });
        }
        if (!results || results.length === 0) {
          return res.status(404).json({ error: { message: 'Account not found' } });
        }
        return res.json(results[0]);
      });
    } catch (err) {
      console.error('getAccount Failed', err);
      return res.status(500).json({ error: { message: 'Server error', details: err.message } });
    }
  },

  createAccounts: async (req, res) => {
    try {
      const accounts = req.body;
      if (!Array.isArray(accounts)) {
        return res.status(400).json({ error: { message: 'Input must be an array of account objects' } });
      }
      const values = accounts.map(a => [a.name, a.email, a.address]);
      connection.query('INSERT INTO accounts (name, email, address) VALUES ?', [values], (err, result) => {
        if (err) {
          console.error('createAccounts Failed', err);
          return res.status(500).json({ error: { message: 'Database error', details: err.message } });
        }
        const insertedId = result.insertId;
        const output = accounts.map((a, idx) => ({ id: insertedId + idx, name: a.name, email: a.email, address: a.address }));
        return res.status(201).json(output);
      });
    } catch (err) {
      console.error('createAccounts Failed', err);
      return res.status(500).json({ error: { message: 'Server error', details: err.message } });
    }
  },

  updateAccount: async (req, res) => {
    try {
      const id = req.params.id;
      const body = req.body;
      // Only update one account at a time
      connection.query('UPDATE accounts SET name = ?, email = ?, address = ? WHERE id = ?', [body.name, body.email, body.address, id], (err, result) => {
        if (err) {
          console.error('updateAccount Failed', err);
          return res.status(500).json({ error: { message: 'Database error', details: err.message } });
        }
        if (result.affectedRows === 0) {
          return res.status(404).json({ error: { message: 'Account not found' } });
        }
        return res.json({ id: Number(id), name: body.name, email: body.email, address: body.address });
      });
    } catch (err) {
      console.error('updateAccount Failed', err);
      return res.status(500).json({ error: { message: 'Server error', details: err.message } });
    }
  },

  deleteAccount: async (req, res) => {
    try {
      const id = req.params.id;
      connection.query('DELETE FROM accounts WHERE id = ?', [id], (err, result) => {
        if (err) {
          console.error('deleteAccount Failed', err);
          return res.status(500).json({ error: { message: 'Database error', details: err.message } });
        }
        if (result.affectedRows === 0) {
          return res.status(404).json({ error: { message: 'Account not found' } });
        }
        return res.json({ deleted: true, id: Number(id) });
      });
    } catch (err) {
      console.error('deleteAccount Failed', err);
      return res.status(500).json({ error: { message: 'Server error', details: err.message } });
    }
  }
};
