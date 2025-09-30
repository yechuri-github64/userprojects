'use strict';

// Structured error handler. Logs errors as JSON and returns a JSON response.
function errorHandler(err, req, res, next) { // eslint-disable-line no-unused-vars
  const status = err.status && Number(err.status) >= 400 && Number(err.status) < 600 ? Number(err.status) : 500;

  const log = {
    timestamp: new Date().toISOString(),
    level: 'error',
    message: err.message || 'Unhandled error',
    path: req.originalUrl,
    method: req.method,
    status,
    validation: err.validation || null,
    meta: err.meta || null,
    stack: process.env.NODE_ENV === 'production' ? undefined : (err.stack || null)
  };

  // Log structured error
  console.error(JSON.stringify(log));

  const clientError = {
    message: err.message || 'Internal Server Error',
    status,
  };
  if (err.validation) clientError.validation = err.validation;
  if (err.meta) clientError.meta = err.meta;

  res.status(status).json(clientError);
}

module.exports = { errorHandler };
