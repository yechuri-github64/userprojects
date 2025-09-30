'use strict';

const Joi = require('joi');

const weatherRequestSchema = Joi.object({
  city: Joi.string().trim().required().messages({ 'any.required': 'city is required' }),
  units: Joi.string().valid('metric', 'imperial').optional(),
  lang: Joi.string().optional()
});

module.exports = { weatherRequestSchema };
