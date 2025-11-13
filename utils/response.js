exports.successResponse = (statusCode, body) => {
  return {
    statusCode: statusCode || 200,
    body: JSON.stringify(body || {})
  };
};

exports.errorResponse = (statusCode, message) => {
  const code = statusCode || 500;
  return {
    statusCode: code,
    body: JSON.stringify({ message: message || 'Error' })
  };
};
