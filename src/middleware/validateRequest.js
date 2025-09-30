'use strict';

const { weatherRequestSchema } = require('../models/weatherModel');

function validateWeatherRequest(req, res, next) {
  const { error, value } = weatherRequestSchema.validate(req.body, { abortEarly: false, stripUnknown: true });
  if (error) {
    const details = error.details.map((d) => ({ message: d.message, path: d.path }));
    const err = new Error('Invalid request');
    err.status = 400;
    err.validation = details;
    return next(err);
  }
  // replace body with validated/normalized value
  req.body = value;
  return next();
}

module.exports = { validateWeatherRequest };
