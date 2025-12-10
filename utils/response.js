module.exports = {
  format: (status, body) => ({ statusCode: status, body: JSON.stringify(body) })
};