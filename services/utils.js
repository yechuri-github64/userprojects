function delay(ms) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

function buildResponse(statusCode, body) {
  const headers = { 'Content-Type': 'application/json' };
  return {
    statusCode,
    headers,
    body: body === null || body === undefined ? '' : JSON.stringify(body)
  };
}

module.exports = { delay, buildResponse };
