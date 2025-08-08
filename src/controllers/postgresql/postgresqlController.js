const { Client } = require('pg');
const pgClient = require('../../connections/postgresql');

exports.getAccounts = async (req, res) => {
  try {
    const result = await pgClient.query('SELECT customer_name, product_name, quantity FROM accounts');
    res.json(result.rows);
  } catch (error) {
    console.error('❌ Failed to retrieve accounts', error);
    res.status(500).json({ error: 'Failed to retrieve accounts' });
  }
};