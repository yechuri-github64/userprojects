module.exports = function(app) {
  try {
    const salesforceRoutes = require('./salesforce/salesforceRoutes');
    app.use('/salesforce/orders', salesforceRoutes);
    console.log('routes/index: Connected');
  } catch (err) {
    console.error('routes/index: Failed', err);
  }
};
