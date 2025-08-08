module.exports = {
  host: process.env.POSTGRESQL_DATABASE_HOST,
  user: process.env.POSTGRESQL_DATABASE_USER,
  password: process.env.POSTGRESQL_DATABASE_PASSWORD,
  database: process.env.POSTGRESQL_DATABASE_NAME,
  ssl: { rejectUnauthorized: false }
};