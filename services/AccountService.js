const sfClient = require('./sfClient');

const SOBJECT = 'sobjects/Account';

const createAccount = async (account) => {
  console.log('Creating account in Salesforce');
  const res = await sfClient.request('post', `${SOBJECT}/`, account);
  // Salesforce create returns { id, success, errors: [] }
  if (res && res.data && res.data.id) {
    const id = res.data.id;
    const created = await getAccount(id);
    return created;
  }
  throw Object.assign(new Error('Failed to create account'), { statusCode: 502, details: res && res.data });
};

const getAccount = async (id) => {
  console.log(`Retrieving account ${id}`);
  try {
    const res = await sfClient.request('get', `${SOBJECT}/${encodeURIComponent(id)}`);
    return res.data;
  } catch (err) {
    if (err.statusCode === 404 || (err.details && err.details[0] && err.details[0].errorCode === 'NOT_FOUND')) return null;
    throw err;
  }
};

const updateAccount = async (id, account) => {
  console.log(`Updating account ${id}`);
  // Salesforce uses PATCH; sfClient will forward method
  await sfClient.request('patch', `${SOBJECT}/${encodeURIComponent(id)}`, account);
  // Retrieve updated record
  const updated = await getAccount(id);
  return updated;
};

const deleteAccount = async (id) => {
  console.log(`Deleting account ${id}`);
  await sfClient.request('delete', `${SOBJECT}/${encodeURIComponent(id)}`);
  return;
};

module.exports = { createAccount, getAccount, updateAccount, deleteAccount };
