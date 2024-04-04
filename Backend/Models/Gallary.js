const mongoose = require('mongoose')
const {Schema} = mongoose

const GallarySchema = new Schema(
    {
        userName: String,
        mode: String, 
        image: Buffer,
        imageType: String
    }
)
const Gallary = mongoose.model('Gallary',GallarySchema)
module.exports = Gallary