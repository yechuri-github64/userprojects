const express = require('express');
const cors = require('cors');
const dotenv = require('dotenv');
try {
  dotenv.config();
  console.log(" Connected");
} catch(err) {
  console.error(" Failed", err);
}
const app = express();
app.use(cors());
app.use(express.json());
try {
  const routes = require('./routes');
  app.use('/api', routes);
  console.log(" Connected");
} catch(err) {
  console.error(" Failed", err);
}
try {
  const errorHandler = require('./middleware/errorHandler');
  app.use(errorHandler);
  console.log(" Connected");
} catch(err) {
  console.error(" Failed", err);
}
try {
  const port = process.env.PORT || 8080;
  app.listen(port, function() {
    console.log(" Connected");
    console.log('Server listening on port ' + port);
  });
} catch(err) {
  console.error(" Failed", err);
}
