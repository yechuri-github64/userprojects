const { app } = require("@azure/functions");

const createItem = require("./src/functions/createItem/index.js");
const listItems = require("./src/functions/listItems/index.js");
const getItem = require("./src/functions/getItem/index.js");
const updateItem = require("./src/functions/updateItem/index.js");
const deleteItem = require("./src/functions/deleteItem/index.js");
const batchCreate = require("./src/functions/batchCreate/index.js");

// Create account
app.http("createItem", {
  methods: ["POST"],
  authLevel: "function",
  route: "accounts",
  handler: createItem,
});

// List accounts
app.http("listItems", {
  methods: ["GET"],
  authLevel: "function",
  route: "accounts",
  handler: listItems,
});

// Get single account
app.http("getItem", {
  methods: ["GET"],
  authLevel: "function",
  route: "accounts/{id}",
  handler: getItem,
});

// Update single account
app.http("updateItem", {
  methods: ["PUT", "PATCH"],
  authLevel: "function",
  route: "accounts/{id}",
  handler: updateItem,
});

// Delete single account
app.http("deleteItem", {
  methods: ["DELETE"],
  authLevel: "function",
  route: "accounts/{id}",
  handler: deleteItem,
});

// Batch create accounts
app.http("batchCreate", {
  methods: ["POST"],
  authLevel: "function",
  route: "accounts/batch",
  handler: batchCreate,
});

module.exports = app;
