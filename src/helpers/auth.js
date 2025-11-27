module.exports = function checkApiKey(req) {
  try {
    const headers = req && (req.headers || {});
    const key = headers["x-api-key"] || headers["X-API-KEY"];
    return key && process.env.API_KEY && key === process.env.API_KEY;
  } catch (e) {
    return false;
  }
};
