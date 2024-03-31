const mongoose = require('mongoose')
const {Schema} = mongoose

const FarmingObjectSchema = new Schema({
    item: {
        type: String, 
        required: true
    },
    startDate: {
        type: String,
    },
    stage: {
        type: Number,
        required: true
    },
    slotnum: {
        type: Number,
        required: true
    }
});

const FarmingSchema = new Schema(
    {
        UserName: {
            type: String,
            required: true,
            unique: true // Ensure unique usernames
        },
        available:{
            type: Number
        },
        farmingObjects: {
            type: [FarmingObjectSchema] // Array of farming objects for each user
        }
    }
)
const Farming = mongoose.model('Farming',FarmingSchema)
module.exports = Farming