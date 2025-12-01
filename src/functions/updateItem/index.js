import { execute } from "../../helpers/mysql/connection.js";

export async function handler(req, context) {
  try {
    const id = req.params && req.params.id;
    if (!id) {
      console.log("updateItem: missing id");
      return {
        status: 400,
        jsonBody: {
          success: false,
          error: { message: "id parameter is required" },
        },
      };
    }

    const body = await req.json();
    const allowed = ["name", "email", "address"];
    const keys = Object.keys(body || {}).filter((k) => allowed.includes(k));
    if (keys.length === 0) {
      return {
        status: 400,
        jsonBody: {
          success: false,
          error: { message: "No updatable fields provided" },
        },
      };
    }

    const setClause = keys.map((k) => `\`${k}\` = ?`).join(", ");
    const params = keys.map((k) => body[k]);
    params.push(id);

    const result = await execute(
      `UPDATE accounts SET ${setClause} WHERE id = ?`,
      params
    );
    // result.affectedRows may be available
    const rows = await execute(
      "SELECT id, name, email, address FROM accounts WHERE id = ?",
      [id]
    );

    if (!rows || rows.length === 0) {
      return {
        status: 404,
        jsonBody: {
          success: false,
          error: { message: "Account not found after update" },
        },
      };
    }

    return {
      status: 200,
      jsonBody: { success: true, data: rows[0] },
    };
  } catch (error) {
    console.log(error);
    return {
      status: 500,
      jsonBody: { success: false, error: { message: error.message } },
    };
  }
}
