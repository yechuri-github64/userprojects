const express = require("express");
const bodyParser = require("body-parser");
const pool = require("../helpers/mysql/connection");
const checkApiKey = require("../helpers/auth");
const responses = require("../helpers/responses");

const app = express();
app.use(bodyParser.json());

// Simple API key middleware for express
app.use((req, res, next) => {
  if (!checkApiKey(req)) {
    res.status(401).json(responses.error("Unauthorized", "unauthorized"));
    return;
  }
  next();
});

// Accounts
app.post("/accounts", async (req, res) => {
  try {
    const { name, email } = req.body || {};
    if (!name || !email)
      return res
        .status(400)
        .json(responses.error("name and email are required", "invalid_input"));
    const [result] = await pool.execute(
      "INSERT INTO accounts (name, email) VALUES (?, ?)",
      [name, email]
    );
    const id = result.insertId;
    const [rows] = await pool.execute(
      "SELECT id, name, email, created_at FROM accounts WHERE id = ?",
      [id]
    );
    res.status(201).json(responses.success(rows[0]));
  } catch (err) {
    console.error("Express create account failed", err);
    res
      .status(500)
      .json(responses.error("Failed to create account", "db_error"));
  }
});

app.get("/accounts", async (req, res) => {
  try {
    const [rows] = await pool.execute(
      "SELECT id, name, email, created_at FROM accounts ORDER BY id DESC"
    );
    res.json(responses.success(rows));
  } catch (err) {
    console.error("Express list accounts failed", err);
    res
      .status(500)
      .json(responses.error("Failed to list accounts", "db_error"));
  }
});

app.get("/accounts/:id", async (req, res) => {
  try {
    const id = req.params.id;
    const [rows] = await pool.execute(
      "SELECT id, name, email, created_at FROM accounts WHERE id = ?",
      [id]
    );
    if (!rows || rows.length === 0)
      return res
        .status(404)
        .json(responses.error("Account not found", "not_found"));
    res.json(responses.success(rows[0]));
  } catch (err) {
    console.error("Express get account failed", err);
    res.status(500).json(responses.error("Failed to get account", "db_error"));
  }
});

app.put("/accounts/:id", async (req, res) => {
  try {
    const id = req.params.id;
    const body = req.body || {};
    const fields = [];
    const values = [];
    if (body.name !== undefined) {
      fields.push("name = ?");
      values.push(body.name);
    }
    if (body.email !== undefined) {
      fields.push("email = ?");
      values.push(body.email);
    }
    if (fields.length === 0)
      return res
        .status(400)
        .json(responses.error("No updatable fields provided", "invalid_input"));
    values.push(id);
    const sql = `UPDATE accounts SET ${fields.join(", ")} WHERE id = ?`;
    const [result] = await pool.execute(sql, values);
    if (result.affectedRows === 0)
      return res
        .status(404)
        .json(responses.error("Account not found", "not_found"));
    const [rows] = await pool.execute(
      "SELECT id, name, email, created_at FROM accounts WHERE id = ?",
      [id]
    );
    res.json(responses.success(rows[0]));
  } catch (err) {
    console.error("Express update account failed", err);
    res
      .status(500)
      .json(responses.error("Failed to update account", "db_error"));
  }
});

app.delete("/accounts/:id", async (req, res) => {
  try {
    const id = req.params.id;
    const [result] = await pool.execute("DELETE FROM accounts WHERE id = ?", [
      id,
    ]);
    if (result.affectedRows === 0)
      return res
        .status(404)
        .json(responses.error("Account not found", "not_found"));
    res.json(responses.success({ id }));
  } catch (err) {
    console.error("Express delete account failed", err);
    res
      .status(500)
      .json(responses.error("Failed to delete account", "db_error"));
  }
});

app.post("/accounts/batch", async (req, res) => {
  const accounts = (req.body && req.body.accounts) || [];
  if (!Array.isArray(accounts) || accounts.length === 0)
    return res
      .status(400)
      .json(responses.error("accounts array is required", "invalid_input"));
  let connection;
  try {
    connection = await pool.getConnection();
    await connection.beginTransaction();
    const inserted = [];
    for (const acc of accounts) {
      const name = acc.name && String(acc.name).trim();
      const email = acc.email && String(acc.email).trim();
      if (!name || !email)
        throw new Error("Each account must have name and email");
      const [result] = await connection.execute(
        "INSERT INTO accounts (name, email) VALUES (?, ?)",
        [name, email]
      );
      const id = result.insertId;
      const [rows] = await connection.execute(
        "SELECT id, name, email, created_at FROM accounts WHERE id = ?",
        [id]
      );
      inserted.push(rows[0]);
    }
    await connection.commit();
    res.status(201).json(responses.success(inserted));
  } catch (err) {
    if (connection)
      try {
        await connection.rollback();
      } catch (e) {
        console.error("Rollback failed", e);
      }
    console.error("Express batch create failed", err);
    res.status(500).json(responses.error("Batch create failed", "db_error"));
  } finally {
    if (connection) connection.release();
  }
});

// Tasks
app.post("/tasks", async (req, res) => {
  try {
    const { title } = req.body || {};
    if (!title)
      return res
        .status(400)
        .json(responses.error("title is required", "invalid_input"));
    const [result] = await pool.execute(
      "INSERT INTO tasks (title, completed) VALUES (?, ?)",
      [title, false]
    );
    const id = result.insertId;
    const [rows] = await pool.execute(
      "SELECT id, title, completed, created_at FROM tasks WHERE id = ?",
      [id]
    );
    res.status(201).json(responses.success(rows[0]));
  } catch (err) {
    console.error("Express create task failed", err);
    res.status(500).json(responses.error("Failed to create task", "db_error"));
  }
});

app.get("/tasks", async (req, res) => {
  try {
    const [rows] = await pool.execute(
      "SELECT id, title, completed, created_at FROM tasks ORDER BY created_at DESC"
    );
    res.json(responses.success(rows));
  } catch (err) {
    console.error("Express list tasks failed", err);
    res.status(500).json(responses.error("Failed to list tasks", "db_error"));
  }
});

app.delete("/tasks/:id", async (req, res) => {
  try {
    const id = req.params.id;
    const [result] = await pool.execute("DELETE FROM tasks WHERE id = ?", [id]);
    if (result.affectedRows === 0)
      return res
        .status(404)
        .json(responses.error("Task not found", "not_found"));
    res.json(responses.success({ id }));
  } catch (err) {
    console.error("Express delete task failed", err);
    res.status(500).json(responses.error("Failed to delete task", "db_error"));
  }
});

module.exports = app;
