const mongoose = require('mongoose')
const {Schema} = mongoose

const FriendListSchema = new Schema(
    {
        UserEmail : {
            type:String,
            required:true,
            unique:true
        },
        Friends:{
            type:Array,
            required:false
        },
        Waitlist:{
            type:Array,
            required:false
        }
    }
)
const FriendList = mongoose.model('FriendList',FriendListSchema)
module.exports = FriendList