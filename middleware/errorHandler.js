'use strict';

let errorHandler;
try {
  errorHandler = (err, req, res, next) => {
    try {
      console.error(" Failed", err);
      const status = err && Number.isInteger(err.status) ? err.status : 500;
      const code = (err && err.code) ? err.code : 'INTERNAL_SERVER_ERROR';
      const message = (err && err.message) ? err.message : 'An unexpected error occurred';
      const details = err && err.details ? err.details : undefined;

      const response = {
        success: false,
        error: {
          message: message,
          code: code
        }
      };
      if (details !== undefined) {
        response.error.details = details;
      }

      console.log(" Connected");
      res.status(status).json(response);
    } catch (handlerErr) {
      console.error(" Failed", handlerErr);
      res.status(500).json({
        success: false,
        error: {
          message: 'Error while processing error',
          code: 'ERROR_HANDLER_FAILURE'
        }
      });
    }
  };
  console.log(" Connected");
} catch (e) {
  console.error(" Failed", e);
  errorHandler = (err, req, res, next) => {
    console.error(" Failed", err);
    res.status(500).json({ success: false, error: { message: 'Critical error', code: 'CRITICAL' } });
  };
}

module.exports = errorHandler;
