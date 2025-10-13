const express = require('express');
const dotenv = require('dotenv');
const cors = require('cors');

try {
  dotenv.config();
  console.log("Connected");
} catch (err) {
  console.error("Failed", err);
}

const app = express();

try {
  app.use(cors());
  app.use(function (req, res, next) {
    res.header("Access-Control-Allow-Origin", "*");
    res.header("Access-Control-Allow-Methods", "GET,PUT,POST,DELETE,OPTIONS");
    res.header("Access-Control-Allow-Headers", "Content-Type, Authorization");
    if (req.method === "OPTIONS") {
      return res.sendStatus(200);
    }
    next();
  });

  app.use(express.json());

  const routes = require('./routes');
  routes(app);

  console.log("Connected");
} catch (err) {
  console.error("Failed", err);
}

const PORT = process.env.BACKEND_PORT || 8080;

try {
  app.listen(PORT, () => {
    console.log("Connected");
    console.log(`Server running on port ${PORT}`);
  });
} catch (err) {
  console.error("Failed", err);
}
