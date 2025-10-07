"use strict";

const express = require("express");

let router;
try {
  router = express.Router();

  const simpleFetch = async (url, timeoutMs = 10000) => {
    if (typeof fetch === "function") {
      const useAbort = typeof AbortController === "function";
      let controller;
      let timer;
      const opts = { headers: { Accept: "application/json" } };
      if (useAbort) {
        controller = new AbortController();
        opts.signal = controller.signal;
        timer = setTimeout(() => controller.abort(), timeoutMs);
      }
      try {
        const res = await fetch(url, opts);
        if (timer) clearTimeout(timer);
        return res;
      } catch (e) {
        if (timer) clearTimeout(timer);
        throw e;
      }
    }
    const https = require("https");
    return await new Promise((resolve, reject) => {
      const req = https.get(url, { headers: { Accept: "application/json" } }, (res) => {
        let data = "";
        res.on("data", (chunk) => (data += chunk));
        res.on("end", () => {
          resolve({
            ok: res.statusCode >= 200 && res.statusCode < 300,
            status: res.statusCode,
            statusText: res.statusMessage,
            text: () => Promise.resolve(data),
            json: () => {
              try {
                return Promise.resolve(JSON.parse(data));
              } catch (err) {
                return Promise.reject(err);
              }
            }
          });
        });
      });
      req.on("error", (err) => reject(err));
      req.setTimeout(timeoutMs, () => {
        req.destroy(new Error("Request timeout"));
      });
    });
  };

  router.get("/guess", async (req, res) => {
    try {
      const name = (req.query && req.query.name ? String(req.query.name) : "").trim();
      if (!name) {
        const errRes = {
          error: {
            code: "BAD_REQUEST",
            message: "Query parameter \"name\" is required",
            details: { queryparameters: { query: { name: "string" } } }
          }
        };
        console.error("Failed", errRes);
        return res.status(400).json(errRes);
      }

      const baseUrl = process.env.AGE_SERVICE_URL;
      if (!baseUrl) {
        const errRes = {
          error: {
            code: "CONFIG_ERROR",
            message: "AGE_SERVICE_URL is not configured",
            details: null
          }
        };
        console.error("Failed", errRes);
        return res.status(500).json(errRes);
      }

      const url = `${baseUrl}?name=${encodeURIComponent(name)}`;

      let response;
      try {
        response = await simpleFetch(url, 10000);
      } catch (fetchErr) {
        const errRes = {
          error: {
            code: "UPSTREAM_UNREACHABLE",
            message: "Failed to reach age service",
            details: String(fetchErr && fetchErr.message ? fetchErr.message : fetchErr)
          }
        };
        console.error("Failed", errRes);
        return res.status(502).json(errRes);
      }

      if (!response.ok) {
        const text = await response.text().catch(() => "");
        const errRes = {
          error: {
            code: "UPSTREAM_ERROR",
            message: "Age service returned an error",
            details: { status: response.status, statusText: response.statusText, body: text }
          }
        };
        console.error("Failed", errRes);
        return res.status(response.status || 502).json(errRes);
      }

      const data = await response.json().catch(() => null);
      if (!data || typeof data !== "object") {
        const errRes = {
          error: {
            code: "PARSE_ERROR",
            message: "Invalid JSON from age service",
            details: null
          }
        };
        console.error("Failed", errRes);
        return res.status(502).json(errRes);
      }

      return res.status(200).json({
        body: data
      });
    } catch (err) {
      const errRes = {
        error: {
          code: "INTERNAL_ERROR",
          message: "An unexpected error occurred",
          details: String(err && err.message ? err.message : err)
        }
      };
      console.error("Failed", errRes);
      return res.status(500).json(errRes);
    }
  });

  console.log("Connected");
} catch (error) {
  console.error("Failed", error);
}

module.exports = router;
