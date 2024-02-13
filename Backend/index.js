require('dotenv').config();
const express = require('express');
const http = require('http');
const session = require('express-session');
const mongoose = require('mongoose');
const WebSocket = require('ws');

const app = express();
const server = http.createServer(app);
const wss = new WebSocket.Server({ server });

const PORT = process.env.PORT || 3000;
const DBUSER = process.env.DBUSER;
const DBPASSWORD = process.env.DBPASSWORD;

app.use(
    session({
        secret: 'Metaverse',
        resave: true,
        saveUninitialized: true,
        collectionName: 'session',
    })
);

app.use(express.urlencoded({ extended: true }));
app.use(express.json());

app.get('/', (req, res) => {
    console.log('local host');
    res.send('Hello World!');
});

const userRoutes = require('./routes/userRoutes');
app.use('/user', userRoutes);

mongoose
    .connect(`mongodb+srv://${DBUSER}:${DBPASSWORD}@metaverse.a2ymtbf.mongodb.net/?retryWrites=true&w=majority`)
    .then(() => {
        server.listen(PORT, () => {
            console.log('Server is running on port ' + PORT);
        });

        // WebSocket handling
        wss.on('connection', (ws) => {
            console.log('WebSocket connection established.');

            // Handle messages from clients
            ws.on('message', (message) => {
                console.log('Received message:', message);

                // You can add your WebSocket logic here
            });

            // Handle disconnection
            ws.on('close', () => {
                console.log('WebSocket connection closed.');
            });
        });
    })
    .catch((error) => {
        console.log('Error: ' + error);
    });
