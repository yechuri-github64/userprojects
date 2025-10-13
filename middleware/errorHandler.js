try {
  function errorHandler(err, req, res, next) {
    try {
      console.error("Failed", err);
      res.status(500).json({ error: { message: 'Internal Server Error' } });
    } catch (inner) {
      console.error("Failed", inner);
      res.status(500).json({ error: { message: 'Unknown error' } });
    }
  }

  console.log("Connected");
  module.exports = errorHandler;
} catch (err) {
  console.error("Failed", err);
  module.exports = function (err, req, res, next) { res.status(500).json({ error: { message: 'Middleware init failed' } }); };
}
