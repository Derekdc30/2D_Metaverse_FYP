const mongoose = require('mongoose')
const {Schema} = mongoose

const InventorySchema = new Schema(
    {
        UserName: {
            type:String,
            required:true,
        },
        Items:{
            type:Array,
            required:false
        },
        Value:{
            type:Array,
            required:false
        }
    }
)
const Inventory = mongoose.model('Inventory',InventorySchema)
module.exports = Inventory