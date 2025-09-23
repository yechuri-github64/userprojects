const { validateOrder, formatError } = require('./utils/helper');
const db = require('./config/db');
require('./config/env');

// Lambda handler name required: ordermanagement
module.exports.ordermanagement = async (event) => {
  try {
    const body = typeof event.body === 'string' ? JSON.parse(event.body) : event.body || event;

    const { valid, errors, data } = validateOrder(body);
    if (!valid) {
      console.error('Validation failed', errors);
      return {
        statusCode: 400,
        body: JSON.stringify({ error: { message: 'Validation failed', details: errors } })
      };
    }

    const { customername, id, order_amount, quantity, subscription } = data;

    const sql = 'INSERT INTO orders (customername, id, order_amount, quantity, subscription) VALUES (?, ?, ?, ?, ?)';
    const params = [customername, id, order_amount, quantity, subscription];

    const result = await db.query(sql, params);

    const insertedId = (result && result.insertId) ? result.insertId : id;

    return {
      statusCode: 201,
      body: JSON.stringify({ message: 'Order saved', data: { insertedId, ...data } })
    };
  } catch (err) {
    console.error('Error saving order', err);
    const formatted = formatError(err);
    return {
      statusCode: formatted.status || 500,
      body: JSON.stringify({ error: formatted })
    };
  }
};
