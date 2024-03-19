const router = require('express').Router()
const User = require('../Models/User')
const FriendList = require('../Models/Friendlist');
const Inventory = require('../Models/Inventory');

router.route('/register').post(async(req,res)=>{
    const name = req.body.name;
    const email  = req.body.email;
    const password  = req.body.password;
    const money = 0;
    const user = { email, name, password, money };
    const userFriendList = {'UserName':name,'Friends':[],'Waitlist':[]};
    const InventoryList = {'UserName':name,'Items':[],'Value':[]};
    if(!name|| !email || !password){
        res.status(400).json({
            message:"Field cannot be empty"
        });
        return
    }

    try {
        const existingUser = await User.findOne({ $or: [{ 'name': name }, { 'email': email }] });
        if (existingUser) {
            res.status(500).json({ error: "Username or email already exists" });
            return;
        }
        await User.collection.insertOne(user);
        await FriendList.collection.insertOne(userFriendList);
        await Inventory.collection.insertOne(InventoryList);
        res.status(200).json({message:"User successfully created!"});
    } catch (error) {
        res.status(500).json({error: error});
    }
});

router.route('/login').post(async(req,res)=>{
    const {email,password} = req.body
    if(!email || !password){
        res.status(400).json({
            message:"Field cannot be empty"
        });
        return
    }

    try {
        const user = await User.findOne({email:email})
        if(user.password == password){
            req.session.logged = true;
            req.session.username = user.name;
            res.status(200).json({name:user.name,email:user.email});
        }
        else{
            res.status(400).json({message:"not valid"});
        }
    } catch (error) {
        res.status(500).json({error: "ERROR"+error});
    }
})

router.route('/me').get(async(req,res)=>{
    if(req.session.logged){
        res.status(200).json(req.session.username);
    }
})

router.route('/FriendList').post(async(req,res)=>{
    const userName = req.body.userName;
    try {
        const list = await FriendList.findOne({UserName:userName});
        res.status(200).json({name:userName, FriendList:list.Friends.toString(), waitlist: list.Waitlist.toString() });
    } catch (error) {
        res.status(500).json({error:"Error: "+ error});
    }
})

router.route('/AddFriend').post(async(req,res)=>{
    const {userName, friendName,mode} = req.body;
    try {
        const friend = await FriendList.findOne({UserName:friendName});
        const currentFriendList = await FriendList.findOne({UserName:userName});
        if (mode == 0) {
            if (!friend) {
                return res.status(501).json({ message: "User not exist" });
            } else if (currentFriendList.Friends.includes(friendName)) {
                return res.status(501).json({ message: "Friend already in the Friend list" });
            } else if (userName == friendName) {
                return res.status(501).json({ message: "That is you!!!" });
            } else if (friend.Waitlist.includes(userName)) {
                return res.status(501).json({ message: "Request already sent!" });
            } else {
                await FriendList.updateOne(
                    { UserName: friendName },
                    { $push: { Waitlist: userName } }
                );
                return res.status(501).json({ message: "Friend Request Sent!" });
            }
        }        
        else if(mode == 1){
            await FriendList.updateOne(
                { UserName: userName },
                { $push: { Friends: friendName}}
            )
            await FriendList.updateOne(
                { UserName: friendName },
                { $push: { Friends: userName}}
            )
            await FriendList.updateOne(
                { UserName: userName },
                { $pull: { Waitlist: friendName }}
            );
        }
        else if(mode == 2){
            await FriendList.updateOne(
                { UserName: userName },
                { $pull: { Waitlist: friendName }}
            );
        }
        else if(mode == 3){
            await FriendList.updateOne(
                { UserName: userName },
                { $pull: { Friends: friendName }}
            );
            await FriendList.updateOne(
                { UserName: friendName },
                { $pull: { Friends: userName }}
            );
        }
        else{
            return res.status(500).json({error:"Unknow Error on Friend List"});
        }
        const list = await FriendList.findOne({UserName:userName});
        return res.status(200).json({UserName:userName, FriendList:list.Friends, waitlist: list.Waitlist});
    } catch (error) {
        return res.status(500).json({error:"Error: "+ error});
    }
})

router.route('/Money').post(async(req,res)=>{
    const Mode = req.body.mode;
    const value = req.body.value;
    const userName = req.body.userName;
    try{
        const user = await User.findOne({name:userName});
        switch(Mode){
            case "1":
                await User.updateOne({name:userName},
                    {$set:{money: user.money+value}});
                return res.status(200).json({message:"add"});
            case "2":
                await User.updateOne({name:userName},
                    {$set:{money: user.money-value}});
                    return res.status(200).json({message:"minus"});
            case "3":
                return res.status(200).json({name:userName, money:user.money});
        }
    } catch(error){
        res.status(500).json({error:"Error: "+error});
    }
})
/*
1 -> update 
2 -> remove 
3 -> get
*/
router.route("/Inventory").post(async(req,res)=>{
    const Mode = req.body.mode;
    const userName = req.body.userName;
    const item = req.body.item;
    const value = req.body.value;
    try {
        // Find the inventory for the specified user
        const inventory = await Inventory.findOne({ UserName: userName });
        switch (Mode) {
            case "1":
                // Check if the item already exists in the array
                const existingIndex = inventory.Items.findIndex((existingItem) => existingItem === item);
                if (existingIndex != -1 ) {
                    // Update the value for the existing item
                    await Inventory.updateOne(
                    { UserName:userName  },
                    { $set: {Value:`${parseInt(value) +parseInt(inventory.Value[existingIndex])}`} }
                    );
                } else {
                    // Add a new item and its value
                    await Inventory.updateOne({UserName:userName},{$push:{Items:item}})
                    await Inventory.updateOne({UserName:userName},{$push:{Value:value}})
                }
                await inventory.save();
                res.status(200).json({ message: 'Inventory updated successfully' });
                break;

            case "2":
                // Remove the item if it exists
                const itemIndexToRemove = inventory.Items.findIndex((existingItem) => existingItem === item);

                if (itemIndexToRemove !== -1) {
                    inventory.Items.splice(itemIndexToRemove, 1);
                    inventory.Value.splice(itemIndexToRemove, 1);
                    await inventory.save();
                    res.status(200).json({ message: 'Item removed from inventory' });
                } else {
                    res.status(404).json({ error: 'Item not found in inventory' });
                }
                break;

            case "3":
                // Get the current inventory
                return res.status(200).json({ Items: inventory.Items.toString(), Value: inventory.Value.toString() });

            default:
                res.status(400).json({ error: 'Invalid mode' });
        }
    } catch (error) {
        console.error('Error updating inventory:', error);
        res.status(500).json({ error: 'Internal server error' });
    }
})

router.route('')
module.exports = router