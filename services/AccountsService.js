<<<<<<< HEAD
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
=======
const DbClient = require('./DbClient');

async function listAccounts() {
  try {
    const rows = await DbClient.query('SELECT id, name, email, address FROM accounts', []);
    return rows;
  } catch (err) {
    console.log('listAccounts error', err);
    throw err;
  }
}

async function getAccount(id) {
  try {
    const rows = await DbClient.query('SELECT id, name, email, address FROM accounts WHERE id = ?', [id]);
    return rows[0] || null;
  } catch (err) {
    console.log('getAccount error', err);
    throw err;
  }
}

async function createAccounts(accounts) {
  if (!Array.isArray(accounts) || accounts.length === 0) return [];
  const values = [];
  for (const a of accounts) {
    const name = a.name || null;
    const email = a.email || null;
    const address = a.address || null;
    values.push([name, email, address]);
  }

  const conn = await DbClient.getConnection();
  try {
    await conn.beginTransaction();
    const placeholders = values.map(() => '(?, ?, ?)').join(', ');
    const flat = values.flat();
    const [result] = await conn.execute(
      `INSERT INTO accounts (name, email, address) VALUES ${placeholders}`,
      flat
    );
    await conn.commit();
    const insertedIds = [];
    let insertId = result.insertId;
    for (let i = 0; i < result.affectedRows; i++) {
      insertedIds.push(insertId + i);
    }
    return insertedIds;
  } catch (err) {
    await conn.rollback();
    console.log('createAccounts error', err);
>>>>>>> bd324d8 (Automated commit on branch accounts-management-lambda from AI2DEV)
    throw err;
  } finally {
    conn.release();
  }
<<<<<<< HEAD
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
=======
}

async function updateAccount(id, data) {
  const allowed = ['name', 'email', 'address'];
  const fields = [];
  const params = [];
  for (const key of allowed) {
    if (Object.prototype.hasOwnProperty.call(data, key)) {
      fields.push(`${key} = ?`);
      params.push(data[key]);
    }
  }
  if (fields.length === 0) return false;
  params.push(id);
  const sql = `UPDATE accounts SET ${fields.join(', ')} WHERE id = ?`;
  try {
    const result = await DbClient.query(sql, params);
    return result.affectedRows && result.affectedRows > 0;
  } catch (err) {
    console.log('updateAccount error', err);
    throw err;
  }
}

async function deleteAccount(id) {
  try {
    const result = await DbClient.query('DELETE FROM accounts WHERE id = ?', [id]);
    return result.affectedRows && result.affectedRows > 0;
  } catch (err) {
    console.log('deleteAccount error', err);
    throw err;
  }
}

module.exports = { listAccounts, getAccount, createAccounts, updateAccount, deleteAccount };
>>>>>>> bd324d8 (Automated commit on branch accounts-management-lambda from AI2DEV)
