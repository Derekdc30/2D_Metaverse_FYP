const router = require('express').Router()
const User = require('../Models/User')
const FriendList = require('../Models/Friendlist');

router.route('/register').post(async(req,res)=>{
    const name = req.body.name;
    const email  = req.body.email;
    const password  = req.body.password;
    const user = { email, name, password };
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
        const friend = await User.findOne({name:friendName});
        const current = await FriendList.findOne({UserName:userName});
        if(!(friend && !current.Friends.includes(friendName) && userName != friendName)){
            res.status(500).json({error:"Error"});
            return;
        }
        if(mode ==0){
            await FriendList.updateOne(
                { UserName: friendName },
                { $push: { Waitlist: userName}}
            )
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
                { UserName: friendName },
                { $pull: { Waitlist: userName }}
            );
        }
        else{
            res.status(500).json({error:"Error: "+ error});
        }
        const list = await FriendList.findOne({UserName:userName});
        res.status(200).json({UserName:userName, FriendList:list.Friends, waitlist: list.Waitlist });
    } catch (error) {
        res.status(500).json({error:"Error: "+ error});
    }
})
router.route('')
module.exports = router