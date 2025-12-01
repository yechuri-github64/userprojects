import { execute } from "../../helpers/mysql/connection.js";

export async function handler(req, context) {
  try {
    const body = await req.json();
    const { name, email, address } = body || {};
    if (!name || !email) {
      console.log("createItem: validation failed");
      return {
        status: 400,
        jsonBody: {
          success: false,
          error: { message: "name and email are required" },
        },
      };
    }

    const insertResult = await execute(
      "INSERT INTO accounts (name, email, address) VALUES (?, ?, ?)",
      [name, email, address || null]
    );
    const id = insertResult.insertId;
    const rows = await execute(
      "SELECT id, name, email, address FROM accounts WHERE id = ?",
      [id]
    );
    const record = Array.isArray(rows) ? rows[0] : rows;

    return {
      status: 201,
      jsonBody: { success: true, data: record },
    };
  } catch (error) {
    console.log(error);
    return {
      status: 500,
      jsonBody: { success: false, error: { message: error.message } },
    };
  }
}
