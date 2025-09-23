function isString(v) { return typeof v === 'string'; }
function isNumber(v) { return typeof v === 'number' && !Number.isNaN(v); }

function validateOrder(input = {}) {
  const errors = [];
  const data = {};

  if (input.customername === undefined || input.customername === null || input.customername === '') {
    errors.push({ field: 'customername', message: 'customername is required' });
  } else if (!isString(input.customername)) {
    errors.push({ field: 'customername', message: 'customername must be a string' });
  } else {
    data.customername = input.customername;
  }

  if (input.id === undefined || input.id === null || input.id === '') {
    errors.push({ field: 'id', message: 'id is required' });
  } else if (!(isString(input.id) || isNumber(input.id))) {
    errors.push({ field: 'id', message: 'id must be a string or number' });
  } else {
    data.id = input.id;
  }

  if (input.order_amount === undefined || input.order_amount === null || input.order_amount === '') {
    errors.push({ field: 'order_amount', message: 'order_amount is required' });
  } else if (!isNumber(Number(input.order_amount))) {
    errors.push({ field: 'order_amount', message: 'order_amount must be a number' });
  } else {
    data.order_amount = Number(input.order_amount);
  }

  if (input.quantity === undefined || input.quantity === null || input.quantity === '') {
    errors.push({ field: 'quantity', message: 'quantity is required' });
  } else if (!Number.isInteger(Number(input.quantity))) {
    errors.push({ field: 'quantity', message: 'quantity must be an integer' });
  } else {
    data.quantity = Number(input.quantity);
  }

  // subscription can be boolean or string representing boolean
  if (input.subscription === undefined || input.subscription === null || input.subscription === '') {
    errors.push({ field: 'subscription', message: 'subscription is required' });
  } else {
    const sub = input.subscription;
    if (typeof sub === 'boolean') {
      data.subscription = sub;
    } else if (isString(sub)) {
      const lowered = sub.toLowerCase();
      if (lowered === 'true' || lowered === 'false') {
        data.subscription = lowered === 'true';
      } else {
        errors.push({ field: 'subscription', message: 'subscription must be boolean or "true"/"false"' });
      }
    } else {
      errors.push({ field: 'subscription', message: 'subscription must be boolean' });
    }
  }

  return { valid: errors.length === 0, errors, data };
}

function formatError(err) {
  const out = {
    message: err && err.message ? String(err.message) : 'Unknown error',
    code: err && err.code ? err.code : undefined,
    details: err && err.stack ? err.stack : undefined,
    status: err && err.status ? err.status : undefined
  };
  return out;
}

module.exports = { validateOrder, formatError };
