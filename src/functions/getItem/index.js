import { execute } from "../../helpers/mysql/connection.js";

export async function handler(req, context) {
  try {
    const id = req.params && req.params.id;
    if (!id) {
      console.log("getItem: missing id");
      return {
        status: 400,
        jsonBody: {
          success: false,
          error: { message: "id parameter is required" },
        },
      };
    }

    const rows = await execute(
      "SELECT id, name, email, address FROM accounts WHERE id = ?",
      [id]
    );
    if (!rows || rows.length === 0) {
      return {
        status: 404,
        jsonBody: { success: false, error: { message: "Account not found" } },
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
