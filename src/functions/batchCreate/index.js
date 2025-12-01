import { getConnection } from "../../helpers/mysql/connection.js";

export async function handler(req, context) {
  let conn;
  try {
    const body = await req.json();
    if (!Array.isArray(body)) {
      console.log("batchCreate: body is not an array");
      return {
        status: 400,
        jsonBody: {
          success: false,
          error: {
            message: "Request body must be an array of account objects",
          },
        },
      };
    }

    conn = await getConnection();
    await conn.beginTransaction();

    const inserted = [];
    for (const item of body) {
      const name = item.name;
      const email = item.email;
      const address = item.address || null;
      if (!name || !email) {
        await conn.rollback();
        return {
          status: 400,
          jsonBody: {
            success: false,
            error: { message: "Each item must have name and email" },
          },
        };
      }
      const [res] = await conn.execute(
        "INSERT INTO accounts (name, email, address) VALUES (?, ?, ?)",
        [name, email, address]
      );
      const id = res.insertId;
      inserted.push({ id, name, email, address });
    }

    await conn.commit();
    return {
      status: 201,
      jsonBody: { success: true, data: inserted },
    };
  } catch (error) {
    if (conn) {
      try {
        await conn.rollback();
      } catch (e) {
        console.log("rollback failed", e);
      }
    }
    console.log(error);
    return {
      status: 500,
      jsonBody: { success: false, error: { message: error.message } },
    };
  } finally {
    if (conn)
      try {
        conn.release();
      } catch (e) {
        console.log("release failed", e);
      }
  }
}
