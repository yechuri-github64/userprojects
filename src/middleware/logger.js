'use strict';

function requestLogger(req, res, next) {
  const start = Date.now();
  const { method, originalUrl } = req;
  const ip = req.ip || req.headers['x-forwarded-for'] || req.connection.remoteAddress;
  res.on('finish', () => {
    const duration = Date.now() - start;
    const log = {
      timestamp: new Date().toISOString(),
      level: 'info',
      msg: 'http_request',
      method,
      path: originalUrl,
      status: res.statusCode,
      durationMs: duration,
      ip
    };
    console.log(JSON.stringify(log));
  });
  next();
}

module.exports = { requestLogger };
