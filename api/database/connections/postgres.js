// Get .env file
require('dotenv').config()

// Initialize postgres database connection
const promise = require('bluebird');
const options = {
    promiseLib: promise
};
const pgp = require('pg-promise')(options);

// Set connection details
const cn = {
    host: process.env.DB_HOST,
    port: 5432,
    database: 'tag',
    user: process.env.DB_USER,
    password: process.env.DB_PASS
};

// Establish connection
const postgres = pgp(cn);

module.exports = postgres;