require('dotenv').config();
const express = require('express');
const http = require('http');
const session = require('express-session');
const mongoose = require('mongoose');
const WebSocket = require('ws');
const multer = require('multer');
const bodyParser = require('body-parser');
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
                ws.send("Greet");
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

    const imageSchema = new mongoose.Schema({
        UserName: String,
        mode: String,
        id:String,
        auction:String,
        image: Buffer,
        contentType: String
      });
      
      // Create a model based on the schema
      const Image = mongoose.model('Image', imageSchema);
      
      // Set up multer for file handling
      const storage = multer.memoryStorage();
      const upload = multer({ storage: storage });
      
      app.use(bodyParser.urlencoded({ extended: true }));
      app.use(bodyParser.json());
      
      // Define the route for uploading images
      app.post('/user/gallary', upload.single('image'), async (req, res) => {
        try {
          const { UserName, mode,id,auction } = req.body;
          const { buffer, mimetype } = req.file;
          if(mode == "0"){
            const newImage = new Image({
            UserName,
            mode,
            id,
            auction,
            image: buffer,
            contentType: mimetype
          });
      
            // Save the image to MongoDB
            await newImage.save();
            console.log("success");
            res.status(200).send('Image uploaded successfully');
          }else{
            await Image.updateOne({id:id},{$set:{auction:auction,UserName:UserName}})
          }
          
        } catch (error) {
          console.error('Error uploading image:', error);
          res.status(500).send('Error uploading image');
        }
      });
      app.post('/user/getid',async(req,res) =>{
        try {
            var ids = await Image.find({auction:true})
            res.status(200).json(ids.id)
        } catch (error) {
            res.status(500).send('Error');
        }
      })
      app.post('/user/getimage', async (req, res) => {
        try {
          const { id } = req.body;
          
          // Find the image by its ID
          const image = await Image.findOne(id);
      
          if (!image) {
            return res.status(404).send('Image not found');
          }
      
          // Set the content type based on the image's contentType
          res.contentType(image.contentType);
      
          // Send the image data
          res.send(image.image);
        } catch (error) {
          console.error('Error retrieving image:', error);
          res.status(500).send('Error retrieving image');
        }
      });