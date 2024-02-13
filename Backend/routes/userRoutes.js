const router = require('express').Router()
const User = require('../Models/User')
const FriendList = require('../Models/Friendlist');

router.route('/register').post(async(req,res)=>{
    const name = req.body.name;
    const email  = req.body.email;
    const password  = req.body.password;
    const money = 0;
    const user = { email, name, password, money };
    const userFriendList = {'UserName':name,'Friends':[],'Waitlist':[]};
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
            return;
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
        const user = await User.findOne({UserName:userName});
        switch(Mode){
            case '1':
                await User.updateOne({UserName:userName},
                    {$set:{Money: user.money+value}});
                res.status(200).json({message:"add"});
                break;
            case '2':
                await User.updateOne({UserName:userName},
                    {$set:{Money: user.money-value}});
                    res.status(200).json({message:"minus"});
                break;
            case '3':
                res.status(200).json({name:userName, money:user.money});
                break;
        }
    } catch(error){
        res.status(500).json({error:"Error: "+error});
    }
})

router.route('')
module.exports = router