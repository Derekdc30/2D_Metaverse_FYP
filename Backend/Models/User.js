const { Int32 } = require('mongodb')
const mongoose = require('mongoose')
const {Schema} = mongoose

const userSchema = new Schema(
    {
        email : {
            type:String,
            required:true,
        },
        name:{
            type:String,
            required:true,
        },
        password:{
            type:String,
            required:true
        },
        money:{
            type:Number,
            required:true
        }
    }
)
const User = mongoose.model('User',userSchema)
module.exports = User