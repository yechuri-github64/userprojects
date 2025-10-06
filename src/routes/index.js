"use strict";

const express = require("express");
const https = require("https");
const { URL } = require("url");

let router;

function safeParseJson(str) {
  try { return JSON.parse(str); } catch (e) { return { raw: String(str || "") }; }
}

function formatError(code, message, details) {
  return { error: { code, message, details: details || null } };
}

async function fetchAgify(name) {
  return new Promise((resolve, reject) => {
    try {
      const base = process.env.AGIFY_API_BASE_URL || "https://api.agify.io";
      const timeout = Number(process.env.AGIFY_REQUEST_TIMEOUT_MS || 5000);
      const url = new URL(base);
      url.searchParams.set("name", name);

      const req = https.get(url.toString(), (res) => {
        let data = "";
        res.on("data", (chunk) => { data += chunk; });
        res.on("end", () => {
          try {
            const status = res.statusCode || 500;
            if (status >= 200 && status < 300) {
              const json = data ? JSON.parse(data) : {};
              resolve(json);
            } else {
              const payload = data ? safeParseJson(data) : null;
              const err = new Error("Upstream service error");
              err.status = status;
              err.details = payload;
              reject(err);
            }
          } catch (parseErr) {
            reject(parseErr);
          }
        });
      });

      req.on("error", (e) => reject(e));
      req.setTimeout(timeout, () => {
        req.destroy(new Error("Request timeout"));
      });
    } catch (err) {
      reject(err);
    }
  });
}

try {
  router = express.Router();

  router.post("/guess-age", async (req, res) => {
    try {
      const body = req.body || {};
      const name = typeof body.name === "string" ? body.name.trim() : "";

      if (!name) {
        console.error("Failed", "Invalid name");
        return res.status(400).json(
          formatError(
            "BAD_REQUEST",
            "Invalid \"name\" provided",
            { expected: "string (non-empty)", received: body && typeof body.name }
          )
        );
      }

      const result = await fetchAgify(name);

      const normalized = {
        name: result && typeof result.name === "string" ? result.name : name,
        age: typeof result.age === "number" ? result.age : null,
        count: typeof result.count === "number" ? result.count : null
      };

      console.log("Connected");
      return res.status(200).json(normalized);
    } catch (err) {
      console.error("Failed", err);
      const status = err.status && Number.isInteger(err.status) ? err.status : 500;
      return res.status(status).json(
        formatError(
          status === 500 ? "INTERNAL_ERROR" : "UPSTREAM_ERROR",
          "Failed to guess age",
          { message: err.message, cause: err.details || null }
        )
      );
    }
  });

  console.log("Connected");
} catch (err) {
  console.error("Failed", err);
  router = express.Router();
  router.use((req, res) => {
    return res.status(500).json(
      formatError("ROUTER_INIT_FAILED", "Failed to initialize routes", { message: err.message })
    );
  });
}

module.exports = router;
