const mysqlConnection = require('../../connections/mysql');

exports.insertOrders = async (req, res) => {
  const orders = req.body;
  const insertedRecords = [];
  try {
    for (const order of orders) {
      const [result] = await mysqlConnection.execute('INSERT INTO orders (`order_number`, `account_id`, `product_name`, `quantity`, `price`, `order_date`, `status`) VALUES (?, ?, ?, ?, ?, ?, ?)', [order.order_number, order.account_id, order.product_name, order.quantity, order.price, order.order_date, order.status]);
      insertedRecords.push({ id: result.insertId, ...order });
    }
    res.json({ status: 'success', insertedRecords });
  } catch (error) {
    console.error('❌ Failed to insert orders', error);
    res.status(500).json({ error: 'Failed to insert orders' });
  }
};