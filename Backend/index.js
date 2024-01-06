require('dotenv').config()
const express = require('express')
const session = require("express-session")
const app = express()
const mongoose = require('mongoose')

const PORT = process.env.PORT || 3000

DBUSER = process.env.DBUSER
DBPASSWORD = process.env.DBPASSWORD

app.use(
    session({
    secret: 'Metaverse',
    resave: true,
    saveUninitialized: true,
    collectionName: 'session',
    
    })
   );

app.use(express.urlencoded({extended:true}))
app.use(express.json())

app.get('/',()=>{
    console.log("local host");
})

const userRoutes = require('./routes/userRoutes')
app.use('/user', userRoutes)

mongoose.connect(`mongodb+srv://${DBUSER}:${DBPASSWORD}@cluster0.jarkwvp.mongodb.net/?retryWrites=true&w=majority`)
.then(()=>{
    app.listen(PORT,()=>{
        console.log("port" + PORT);
    })
})
.catch(error =>{
    console.log("Error: "+ error);
})