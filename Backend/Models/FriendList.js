const mongoose = require('mongoose')
const {Schema} = mongoose

const FriendListSchema = new Schema(
    {
        Username : {
            type:String,
            required:true,
            unique:true
        },
        Friends:{
            type:String,
            required:true
        },
        Waitlist:{
            type:String,
            required:true
        }
    }
)
const FriendLists = mongoose.model('FriendList',FriendListSchema)
module.exports = FriendLists