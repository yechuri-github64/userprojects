try {
  function errorHandler(err, req, res, next) {
    try {
      console.error(" Failed", err);
      const status = err && err.status ? err.status : 500;
      const payload = {
        error: {
          message: err && err.message ? err.message : 'Internal Server Error',
          status: status,
          details: err && err.details ? err.details : null
        }
      };
      res.status(status).json(payload);
    } catch(inner) {
      console.error(" Failed", inner);
      res.status(500).json({ error: { message: 'Internal Server Error', status: 500 } });
    }
  }
  module.exports = errorHandler;
  console.log(" Connected");
} catch(err) {
  console.error(" Failed", err);
}
