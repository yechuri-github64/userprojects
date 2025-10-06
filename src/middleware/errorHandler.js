let handler;
try {
  handler = (err, req, res, next) => {
    try {
      console.error(' Failed', err);
      const status = err.status && Number.isInteger(err.status) ? err.status : 500;
      const payload = {
        success: false,
        error: {
          code: err.code || 'INTERNAL_ERROR',
          message: err.message || 'Internal Server Error',
          details: err.details || null
        }
      };
      res.status(status).json(payload);
    } catch (e) {
      console.error(' Failed', e);
      res.status(500).json({
        success: false,
        error: { code: 'INTERNAL_ERROR', message: 'Internal Server Error', details: null }
      });
    }
  };
  console.log(' Connected');
} catch (err) {
  console.error(' Failed', err);
  handler = (err, req, res, next) => {
    res.status(500).json({ success: false, error: { code: 'INTERNAL_ERROR', message: 'Internal Server Error', details: null } });
  };
}

module.exports = handler;
