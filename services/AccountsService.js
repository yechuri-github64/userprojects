require('dotenv').config();
const db = require('../utils/db');
module.exports = {
  getAll: async () => {
    try {
      const [rows] = await db.query('SELECT id, name, email, address FROM accounts');
      return rows;
    } catch (err) {
      console.log('getAll error', err);
      throw err;
    }
  },
  getById: async (id) => {
    try {
      const [rows] = await db.query('SELECT id, name, email, address FROM accounts WHERE id = ?', [id]);
      return rows[0] || null;
    } catch (err) {
      console.log('getById error', err);
      throw err;
    }
  },
  createMultiple: async (accounts) => {
    try {
      if (!Array.isArray(accounts) || accounts.length === 0) throw new Error('No accounts to insert');
      const values = accounts.map(a => [a.name || null, a.email || null, a.address || null]);
      const [result] = await db.query('INSERT INTO accounts (name, email, address) VALUES ?', [values]);
      return result;
    } catch (err) {
      console.log('createMultiple error', err);
      throw err;
    }
  },
  updateOne: async (id, payload) => {
    try {
      const allowed = ['name', 'email', 'address'];
      const fields = [];
      const params = [];
      for (const key of allowed) {
        if (Object.prototype.hasOwnProperty.call(payload, key)) {
          fields.push(`${key} = ?`);
          params.push(payload[key]);
        }
      }
      if (fields.length === 0) throw new Error('No valid fields to update');
      params.push(id);
      const sql = `UPDATE accounts SET ${fields.join(', ')} WHERE id = ?`;
      const [result] = await db.query(sql, params);
      return result;
    } catch (err) {
      console.log('updateOne error', err);
      throw err;
    }
  },
  deleteOne: async (id) => {
    try {
      const [result] = await db.query('DELETE FROM accounts WHERE id = ?', [id]);
      return result;
    } catch (err) {
      console.log('deleteOne error', err);
      throw err;
    }
  }
};