const dotenv = require('dotenv');
const path = require('path');

// load .env from project root
dotenv.config({ path: path.resolve(process.cwd(), '.env') });

// Expose the environment variables used by the DB connector
module.exports = {
  DATABASE: process.env.DATABASE,
  DATABASE_HOST: process.env.DATABASE_HOST,
  DATABASE_USER: process.env.DATABASE_USER,
  DATABASE_PASSWORD: process.env.DATABASE_PASSWORD,
  DATABASE_NAME: process.env.DATABASE_NAME
};
