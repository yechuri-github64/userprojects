const db = require('./db');

const table = 'accounts';

const getAllAccounts = async () => {
  try {
    console.log('Fetching all accounts');
    const rows = await db.execute(`SELECT id, name, email, address FROM ${table}`);
    return rows;
  } catch (err) {
    console.log('getAllAccounts error:', err);
    throw err;
  }
};

const getAccount = async (id) => {
  try {
    console.log('Fetching account id=', id);
    const rows = await db.execute(`SELECT id, name, email, address FROM ${table} WHERE id = ?`, [id]);
    return rows[0] || null;
  } catch (err) {
    console.log('getAccount error:', err);
    throw err;
  }
};

const createAccounts = async (accounts) => {
  if (!Array.isArray(accounts) || accounts.length === 0) throw new Error('Accounts array required');
  const pool = db.getPool();
  const conn = await pool.getConnection();
  try {
    await conn.beginTransaction();
    const values = [];
    const params = [];
    for (const acc of accounts) {
      // allow missing fields but ensure name/email/address keys exist as string or null
      const name = acc.name !== undefined ? acc.name : null;
      const email = acc.email !== undefined ? acc.email : null;
      const address = acc.address !== undefined ? acc.address : null;
      values.push('(?, ?, ?)');
      params.push(name, email, address);
    }
    const sql = `INSERT INTO ${table} (name, email, address) VALUES ${values.join(',')}`;
    const [result] = await conn.query(sql, params);
    await conn.commit();
    return { affectedRows: result.affectedRows, insertedCount: accounts.length, insertId: result.insertId };
  } catch (err) {
    await conn.rollback();
    console.log('createAccounts error:', err);
    throw err;
  } finally {
    conn.release();
  }
};

const updateAccount = async (id, data) => {
  if (!id) throw new Error('id is required for update');
  const allowed = ['name', 'email', 'address'];
  const sets = [];
  const params = [];
  for (const key of allowed) {
    if (Object.prototype.hasOwnProperty.call(data, key)) {
      sets.push(`${key} = ?`);
      params.push(data[key]);
    }
  }
  if (sets.length === 0) throw new Error('No updatable fields provided');
  params.push(id);
  try {
    console.log('Updating account', id);
    const rows = await db.execute(`UPDATE ${table} SET ${sets.join(', ')} WHERE id = ?`, params);
    // rows is a ResultSetHeader-like object when using execute via pool
    return rows;
  } catch (err) {
    console.log('updateAccount error:', err);
    throw err;
  }
};

const deleteAccount = async (id) => {
  if (!id) throw new Error('id is required for delete');
  try {
    console.log('Deleting account', id);
    const rows = await db.execute(`DELETE FROM ${table} WHERE id = ?`, [id]);
    return rows;
  } catch (err) {
    console.log('deleteAccount error:', err);
    throw err;
  }
};

module.exports = { getAllAccounts, getAccount, createAccounts, updateAccount, deleteAccount };