try {
  // Centralized error handler middleware
  module.exports = (err, req, res, next) => {
    try {
      const status = err && err.status ? err.status : 500;
      const payload = {
        error: {
          message: err && err.message ? err.message : 'Internal server error',
          code: err && err.code ? err.code : 'INTERNAL_ERROR',
          details: err && err.details ? err.details : null
        }
      };
      console.error('Failed', err);
      res.status(status).json(payload);
    } catch (handlerErr) {
      console.error('Failed', handlerErr);
      res.status(500).json({ error: { message: 'Error handler failure', code: 'ERROR_HANDLER_FAILURE', details: handlerErr && handlerErr.message ? handlerErr.message : handlerErr } });
    }
  };

  console.log('Connected');
} catch (err) {
  console.error('Failed', err);
  module.exports = (err, req, res, next) => res.status(500).json({ error: { message: 'Middleware failed to load', code: 'MIDDLEWARE_LOAD_ERROR', details: err && err.message ? err.message : err } });
}
