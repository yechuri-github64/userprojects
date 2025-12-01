import { execute } from "../../helpers/mysql/connection.js";

export async function handler(req, context) {
  try {
    const id = req.params && req.params.id;
    if (!id) {
      console.log("deleteItem: missing id");
      return {
        status: 400,
        jsonBody: {
          success: false,
          error: { message: "id parameter is required" },
        },
      };
    }

    const result = await execute("DELETE FROM accounts WHERE id = ?", [id]);
    const affected =
      result.affectedRows !== undefined
        ? result.affectedRows
        : result.affectedRows || 0;

    if (!affected) {
      return {
        status: 404,
        jsonBody: { success: false, error: { message: "Account not found" } },
      };
    }

    return {
      status: 200,
      jsonBody: { success: true, data: { id } },
    };
  } catch (error) {
    console.log(error);
    return {
      status: 500,
      jsonBody: { success: false, error: { message: error.message } },
    };
  }
}
