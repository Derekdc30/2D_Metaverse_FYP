const mongoose = require('mongoose')
const {Schema} = mongoose

const FriendListSchema = new Schema(
    {
        UserName: {
            type:String,
            required:true,
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