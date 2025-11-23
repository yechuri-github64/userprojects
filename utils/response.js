exports.success = function(statusCode, body) {
  return {
    statusCode: statusCode || 200,
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body)
  };
};

exports.failure = function(statusCode, body) {
  return {
    statusCode: statusCode || 500,
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body)
  };
};
