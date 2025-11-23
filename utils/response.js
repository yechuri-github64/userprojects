function parseJson(input) {
  try {
    if (!input && input !== '') return null;
    return typeof input === 'object' ? input : JSON.parse(input);
  } catch (err) {
    console.error('parseJson error', err);
    return null;
  }
}

function successResponse(statusCode, data) {
  return {
    statusCode: statusCode || 200,
    body: JSON.stringify(data || {})
  };
}

function errorResponse(statusCode, message) {
  const code = statusCode || 500;
  return {
    statusCode: code,
    body: JSON.stringify({ error: message || 'Error' })
  };
}

module.exports = { parseJson, successResponse, errorResponse };
