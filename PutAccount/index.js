const { execute } = require("../src/helpers/mysql/connection");

module.exports = async function (context, req) {
  context.log("PutAccount invoked");
  try {
    const id = req.params && req.params.id;
    if (!id) {
      context.res = { status: 400, body: { error: "Missing id parameter" } };
      return;
    }

    const body = req.body;
    if (
      !body ||
      (body.name === undefined &&
        body.email === undefined &&
        body.address === undefined)
    ) {
      context.res = { status: 400, body: { error: "No fields to update" } };
      return;
    }

    const fields = [];
    const params = [];

    if (body.name !== undefined) {
      fields.push("name = ?");
      params.push(body.name);
    }
    if (body.email !== undefined) {
      fields.push("email = ?");
      params.push(body.email);
    }
    if (body.address !== undefined) {
      fields.push("address = ?");
      params.push(body.address);
    }

    if (fields.length === 0) {
      context.res = {
        status: 400,
        body: { error: "No valid fields to update" },
      };
      return;
    }

    params.push(id);
    const sql = "UPDATE accounts SET " + fields.join(", ") + " WHERE id = ?";
    const result = await execute(sql, params);

    if (!result || result.affectedRows === 0) {
      context.res = { status: 404, body: { error: "Account not found" } };
      return;
    }

    context.res = { status: 200, body: { id: Number(id), ...body } };
  } catch (err) {
    context.log.error(err);
    context.res = {
      status: 500,
      body: { error: "Internal Server Error", details: err.message },
    };
  }
};
