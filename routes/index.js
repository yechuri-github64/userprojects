module.exports = function(app) {
  try {
    const accountsRouter = require('./accounts');
    app.use('/accounts', accountsRouter);
    console.log("Connected");
  } catch (err) {
    console.error("Failed", err);
    throw err;
  }
};
