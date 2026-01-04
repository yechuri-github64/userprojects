const conn = require('../../connections/salesforce');

// Helper to format errors
function formatError(err) {
  return { message: err.message || 'Unknown error', details: err }
}

// Sample input-output mapping examples (for reference):
// Create input:
// {
//   "firstName": "John",
//   "lastName": "Doe",
//   "status": "Draft",
//   "accountId": "001xxxxxxxxxxxx",
//   "effectiveDate": "2026-01-01"
// }
// Create output:
// {
//   "id": "a0Dxxxxxxxxxxxx",
//   "success": true
// }

exports.createOrder = async (req, res) => {
  try {
    const body = req.body || {};
    const firstName = (body.firstName || '').toString();
    const lastName = (body.lastName || '').toString();

    // Ensure Name has first name always with a hyphen
    const Name = `${firstName}-${lastName}`;

    const orderRecord = {
      Name: Name,
      Status: body.status || 'Draft',
      AccountId: body.accountId,
      EffectiveDate: body.effectiveDate
    };

    const result = await conn.sobject('Order').create(orderRecord);

    if (!result || !result.success) {
      console.error('createOrder: Failed to create', result);
      return res.status(400).json({ error: { message: 'Failed to create order', details: result } });
    }

    res.status(201).json({ request: orderRecord, response: { id: result.id, success: result.success } });
  } catch (err) {
    console.error('createOrder: Failed', err);
    res.status(500).json({ error: formatError(err) });
  }
};

exports.getOrders = async (req, res) => {
  try {
    // Select common Order fields; adjust as necessary
    const query = "SELECT Id, Name, Status, EffectiveDate, AccountId FROM Order ORDER BY CreatedDate DESC LIMIT 200";
    const result = await conn.query(query);
    res.json({ records: result.records });
  } catch (err) {
    console.error('getOrders: Failed', err);
    res.status(500).json({ error: formatError(err) });
  }
};

exports.getOrderById = async (req, res) => {
  try {
    const id = req.params.id;
    const record = await conn.sobject('Order').retrieve(id);
    if (!record || record.errorCode) {
      console.error('getOrderById: Not found or error', record);
      return res.status(404).json({ error: { message: 'Order not found', details: record } });
    }
    res.json({ record });
  } catch (err) {
    console.error('getOrderById: Failed', err);
    res.status(500).json({ error: formatError(err) });
  }
};

exports.updateOrder = async (req, res) => {
  try {
    const id = req.params.id;
    const body = req.body || {};

    // If updating name, ensure firstName hyphen rule if firstName provided
    if (body.firstName) {
      body.Name = `${body.firstName}-${body.lastName || ''}`;
    }

    // Map allowed updatable fields for safety
    const updateFields = {};
    if (body.Name) updateFields.Name = body.Name;
    if (body.Status) updateFields.Status = body.Status;
    if (body.EffectiveDate || body.effectiveDate) updateFields.EffectiveDate = body.EffectiveDate || body.effectiveDate;
    if (body.AccountId || body.accountId) updateFields.AccountId = body.AccountId || body.accountId;

    updateFields.Id = id;

    const result = await conn.sobject('Order').update(updateFields);
    if (!result || result.success === false) {
      console.error('updateOrder: Failed to update', result);
      return res.status(400).json({ error: { message: 'Failed to update order', details: result } });
    }
    res.json({ request: updateFields, response: result });
  } catch (err) {
    console.error('updateOrder: Failed', err);
    res.status(500).json({ error: formatError(err) });
  }
};

exports.deleteOrder = async (req, res) => {
  try {
    const id = req.params.id;
    const result = await conn.sobject('Order').destroy(id);
    if (!result || result.success === false) {
      console.error('deleteOrder: Failed to delete', result);
      return res.status(400).json({ error: { message: 'Failed to delete order', details: result } });
    }
    res.json({ response: result });
  } catch (err) {
    console.error('deleteOrder: Failed', err);
    res.status(500).json({ error: formatError(err) });
  }
};
