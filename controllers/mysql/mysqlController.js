const connection = require('../../connections/mysql');

async function getAccount(req, res) {
  try {
    const id = parseInt(req.params.id, 10);
    if (isNaN(id)) {
      return res.status(400).json({ success: false, error: { message: 'Invalid id' } });
    }

    connection.query('SELECT id, name, email, address FROM accounts WHERE id = ?', [id], (err, results) => {
      try {
        if (err) {
          console.error('Failed', err);
          return res.status(500).json({ success: false, error: { message: err.message } });
        }
        if (!results || results.length === 0) {
          return res.status(404).json({ success: false, error: { message: 'Account not found' } });
        }
        const row = results[0];
        return res.json({ id: row.id, name: row.name, email: row.email, address: row.address });
      } catch (errInner) {
        console.error('Failed', errInner);
        return res.status(500).json({ success: false, error: { message: errInner.message } });
      }
    });
  } catch (err) {
    console.error('Failed', err);
    return res.status(500).json({ success: false, error: { message: err.message } });
  }
}

async function createAccounts(req, res) {
  try {
    // Accept either raw array in body or body.queryparameters as per sample
    const payload = Array.isArray(req.body) ? req.body : req.body.queryparameters || req.body.input || [];

    if (!Array.isArray(payload) || payload.length === 0) {
      return res.status(400).json({ success: false, error: { message: 'No accounts provided' } });
    }

    const values = [];
    for (const item of payload) {
      const name = item.name || null;
      const email = item.email || null;
      const address = item.address || null;
      values.push([name, email, address]);
    }

    connection.query('INSERT INTO accounts (name, email, address) VALUES ?', [values], (err, result) => {
      try {
        if (err) {
          console.error('Failed', err);
          return res.status(500).json({ success: false, error: { message: err.message } });
        }

        const insertedId = result.insertId;
        const insertedCount = result.affectedRows;
        const response = [];
        for (let i = 0; i < insertedCount; i++) {
          response.push({ id: insertedId + i, name: values[i][0], email: values[i][1], address: values[i][2] });
        }
        return res.status(201).json(response);
      } catch (errInner) {
        console.error('Failed', errInner);
        return res.status(500).json({ success: false, error: { message: errInner.message } });
      }
    });
  } catch (err) {
    console.error('Failed', err);
    return res.status(500).json({ success: false, error: { message: err.message } });
  }
}

async function updateAccount(req, res) {
  try {
    const id = parseInt(req.params.id, 10);
    if (isNaN(id)) {
      return res.status(400).json({ success: false, error: { message: 'Invalid id' } });
    }

    // Accept fields from body.queryparameters or body.input
    const payload = req.body.queryparameters || req.body.input || req.body || {};
    const fields = {};
    if (payload.name) fields.name = payload.name;
    if (payload.email) fields.email = payload.email;
    if (payload.address) fields.address = payload.address;

    if (Object.keys(fields).length === 0) {
      return res.status(400).json({ success: false, error: { message: 'No fields to update' } });
    }

    const sets = [];
    const values = [];
    for (const key of Object.keys(fields)) {
      sets.push(`${key} = ?`);
      values.push(fields[key]);
    }
    values.push(id);

    const sql = `UPDATE accounts SET ${sets.join(', ')} WHERE id = ?`;

    connection.query(sql, values, (err, result) => {
      try {
        if (err) {
          console.error('Failed', err);
          return res.status(500).json({ success: false, error: { message: err.message } });
        }
        // Return updated row
        connection.query('SELECT id, name, email, address FROM accounts WHERE id = ?', [id], (err2, results) => {
          try {
            if (err2) {
              console.error('Failed', err2);
              return res.status(500).json({ success: false, error: { message: err2.message } });
            }
            if (!results || results.length === 0) {
              return res.status(404).json({ success: false, error: { message: 'Account not found after update' } });
            }
            const row = results[0];
            return res.json({ id: row.id, name: row.name, email: row.email, address: row.address });
          } catch (errInner) {
            console.error('Failed', errInner);
            return res.status(500).json({ success: false, error: { message: errInner.message } });
          }
        });
      } catch (errInner) {
        console.error('Failed', errInner);
        return res.status(500).json({ success: false, error: { message: errInner.message } });
      }
    });
  } catch (err) {
    console.error('Failed', err);
    return res.status(500).json({ success: false, error: { message: err.message } });
  }
}

async function deleteAccount(req, res) {
  try {
    const id = parseInt(req.params.id, 10);
    if (isNaN(id)) {
      return res.status(400).json({ success: false, error: { message: 'Invalid id' } });
    }

    connection.query('DELETE FROM accounts WHERE id = ?', [id], (err, result) => {
      try {
        if (err) {
          console.error('Failed', err);
          return res.status(500).json({ success: false, error: { message: err.message } });
        }
        if (result.affectedRows === 0) {
          return res.status(404).json({ success: false, error: { message: 'Account not found' } });
        }
        return res.json({ success: true, deletedId: id });
      } catch (errInner) {
        console.error('Failed', errInner);
        return res.status(500).json({ success: false, error: { message: errInner.message } });
      }
    });
  } catch (err) {
    console.error('Failed', err);
    return res.status(500).json({ success: false, error: { message: err.message } });
  }
}

module.exports = { getAccount, createAccounts, updateAccount, deleteAccount };
