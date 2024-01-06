const router = require('express').Router()
const User = require('../Models/User')
const FriendList = require('../Models/Friendlist');

router.route('/register').post(async(req,res)=>{
    const {name, email, password} = req.body
    const user = {name,email,password}
    const userFriendList = {UserEmail:email,Friends:[],Waitlist:[]}
    if(!name|| !email || !password){
        res.status(400).json({
            message:"Field cannot be empty"
        });
        return
    }

    try {
        const existingUser = await User.findOne({ $or: [{ name: name }, { email: email }] });
        if (existingUser) {
            res.status(500).json({ error: "Username or email already exists" });
            return;
        }
        await User.create(user)
        await FriendList.create(userFriendList);
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
    const userEmail = req.body.userEmail;
    try {
        const list = await FriendList.findOne({UserEmail:userEmail});
        res.status(200).json({email:userEmail, FriendList:list.Friends.toString(), waitlist: list.Waitlist.toString() });
    } catch (error) {
        res.status(500).json({error:"Error: "+ error});
    }
})

router.route('/AddFriend').post(async(req,res)=>{
    const {userEmail, friendName} = req.body;
    try {
        const friend = await User.findOne({name:friendName});
        const current = await FriendList.findOne({UserEmail:userEmail});
        if(friend && !current.Friends.includes(friendName)){
            await FriendList.updateOne(
                { UserEmail: userEmail },
                { $push: { Friends: friendName}}
             )
        }
        else{
            res.status(500).json({error:"friend exist"});
            return;
        }
        const list = await FriendList.findOne({UserEmail:userEmail});
        res.status(200).json({email:userEmail, FriendList:list.Friends, waitlist: list.Waitlist });
    } catch (error) {
        res.status(500).json({error:"Error: "+ error});
    }
})
router.route('')
module.exports = router