"use strict";

require("dotenv").config();

const express = require("express");
const cors = require("cors");

try {
  const app = express();

  app.use(
    cors({
      origin: "*",
      methods: ["GET", "HEAD", "PUT", "PATCH", "POST", "DELETE"],
      allowedHeaders: ["Content-Type", "Authorization", "Accept"]
    })
  );

  app.use(express.json({ limit: "1mb" }));

  const routes = require("./routes");
  app.use("/api", routes);

  app.use((req, res) => {
    try {
      return res.status(404).json({
        error: {
          code: "NOT_FOUND",
          message: "Route not found",
          details: { method: req.method, url: req.originalUrl }
        }
      });
    } catch (err) {
      console.error("Failed", err);
      return res.status(500).json({
        error: {
          code: "INTERNAL_ERROR",
          message: "Unexpected error",
          details: { message: err.message }
        }
      });
    }
  });

  app.use((err, req, res, next) => {
    console.error("Failed", err);
    res.status(err.status || 500).json({
      error: {
        code: err.code || "INTERNAL_ERROR",
        message: err.message || "Unexpected error",
        details: err.details || null
      }
    });
  });

  const PORT = process.env.PORT || 3000;

  app.listen(PORT, () => {
    console.log("Connected");
  });

  process.on("unhandledRejection", (reason) => {
    console.error("Failed", reason);
  });

  process.on("uncaughtException", (err) => {
    console.error("Failed", err);
  });
} catch (err) {
  console.error("Failed", err);
}
