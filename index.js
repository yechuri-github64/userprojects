"use strict";

try {
  const express = require("express");
  const cors = require("cors");
  const dotenv = require("dotenv");

  dotenv.config();

  const app = express();

  app.use(cors({
    origin: "*",
    methods: ["GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS"],
    allowedHeaders: ["Content-Type", "Authorization", "Accept"]
  }));

  app.use(express.json());
  app.use(express.urlencoded({ extended: true }));

  app.use(require("./routes"));

  app.use((req, res) => {
    const errRes = {
      error: {
        code: "NOT_FOUND",
        message: "Resource not found",
        details: { path: req.originalUrl, method: req.method }
      }
    };
    console.error("Failed", errRes);
    res.status(404).json(errRes);
  });

  const PORT = process.env.SERVER_PORT || 8080;

  app.listen(PORT, () => {
    console.log("Connected");
    console.log(`Server running on port ${PORT}`);
  });
} catch (error) {
  console.error("Failed", error);
  process.exit(1);
}
