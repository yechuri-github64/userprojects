const { app } = require("@azure/functions");
const createItem = require("./src/functions/createItem/index.js");
const listItems = require("./src/functions/listItems/index.js");
const getItem = require("./src/functions/getItem/index.js");
const updateItem = require("./src/functions/updateItem/index.js");
const deleteItem = require("./src/functions/deleteItem/index.js");
const batchCreate = require("./src/functions/batchCreate/index.js");

// Register HTTP functions using Azure Functions v4 programming model
app.http("createItem", {
  methods: ["POST"],
  authLevel: "function",
  route: "accounts",
  handler: createItem,
});

app.http("listItems", {
  methods: ["GET"],
  authLevel: "function",
  route: "accounts",
  handler: listItems,
});

app.http("getItem", {
  methods: ["GET"],
  authLevel: "function",
  route: "accounts/{id}",
  handler: getItem,
});

app.http("updateItem", {
  methods: ["PUT"],
  authLevel: "function",
  route: "accounts/{id}",
  handler: updateItem,
});

app.http("deleteItem", {
  methods: ["DELETE"],
  authLevel: "function",
  route: "accounts/{id}",
  handler: deleteItem,
});

app.http("batchCreate", {
  methods: ["POST"],
  authLevel: "function",
  route: "accounts/batch",
  handler: batchCreate,
});

module.exports = app;
