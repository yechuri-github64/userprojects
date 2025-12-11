const ageService = require('../services/ageService');
module.exports = {
  handle: async (event, context) => {
    console.log('UserHandler invoked', JSON.stringify({ event: event }));
    try {
      const body = typeof event.body === 'string' && event.body ? JSON.parse(event.body) : (event.body || event);
      if (!body || !body.dob) {
        const errorResponse = { message: 'Missing required field: dob' };
        console.log('Validation error', JSON.stringify(errorResponse));
        return {
          statusCode: 400,
          body: JSON.stringify({ success: false, error: errorResponse })
        };
      }
      const result = await ageService.calculateFromDOB(body.dob);
      return {
        statusCode: 200,
        body: JSON.stringify({ success: true, data: result })
      };
    } catch (err) {
      console.log('Handler error', err && err.message ? err.message : err);
      const statusCode = err && err.statusCode ? err.statusCode : 500;
      return {
        statusCode,
        body: JSON.stringify({ success: false, error: { message: err && err.message ? err.message : 'Internal server error' } })
      };
    }
  }
};