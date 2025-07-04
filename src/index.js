require('dotenv').config();
const express = require('express');
const cors = require('cors');
const app = express();

// 🔓 CORS setup - all origins, all methods
app.use(cors({
    origin: '*',
    methods: ['GET', 'POST', 'PUT', 'DELETE', 'PATCH', 'OPTIONS'],
    allowedHeaders: ['Content-Type', 'Authorization']
}));

app.use(express.json());

const routes = require('./routes');
app.use('/api', routes);

const PORT = process.env.SERVER_PORT || 3000;
app.listen(PORT, () => {
    console.log(`Server is running on port ${PORT}`);
});
