import { execute } from "../../helpers/mysql/connection.js";

export async function handler(req, context) {
  try {
    const rows = await execute(
      "SELECT id, name, email, address FROM accounts",
      []
    );
    return {
      status: 200,
      jsonBody: { success: true, data: rows },
    };
  } catch (error) {
    console.log(error);
    return {
      status: 500,
      jsonBody: { success: false, error: { message: error.message } },
    };
  }
}
