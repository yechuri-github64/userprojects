import { app } from "@azure/functions";
import { handler as createItemHandler } from "./src/functions/createItem/index.js";
import { handler as listItemsHandler } from "./src/functions/listItems/index.js";
import { handler as getItemHandler } from "./src/functions/getItem/index.js";
import { handler as updateItemHandler } from "./src/functions/updateItem/index.js";
import { handler as deleteItemHandler } from "./src/functions/deleteItem/index.js";
import { handler as batchCreateHandler } from "./src/functions/batchCreate/index.js";

export const createItem = app.http("createItem", {
  methods: ["POST"],
  authLevel: "function",
  route: "accounts",
  handler: createItemHandler,
});

export const listItems = app.http("listItems", {
  methods: ["GET"],
  authLevel: "function",
  route: "accounts",
  handler: listItemsHandler,
});

export const getItem = app.http("getItem", {
  methods: ["GET"],
  authLevel: "function",
  route: "accounts/{id}",
  handler: getItemHandler,
});

export const updateItem = app.http("updateItem", {
  methods: ["PUT", "PATCH"],
  authLevel: "function",
  route: "accounts/{id}",
  handler: updateItemHandler,
});

export const deleteItem = app.http("deleteItem", {
  methods: ["DELETE"],
  authLevel: "function",
  route: "accounts/{id}",
  handler: deleteItemHandler,
});

export const batchCreate = app.http("batchCreate", {
  methods: ["POST"],
  authLevel: "function",
  route: "accounts/batch",
  handler: batchCreateHandler,
});
