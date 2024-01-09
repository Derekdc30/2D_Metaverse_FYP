using System.Collections;
using System.Collections.Generic;
using FishNet.Object.Synchronizing;
using FishNet.Object;
using System;

//ChatManager is a script that manages the chat system in the game. It is attached to the ChatManager prefab.
//It will be used to send messages from the input field to the chat.

public sealed class ChatManager : NetworkBehaviour
{
    [SyncObject]
    private readonly SyncList<MessageData> _messages = new();

    public IReadOnlyList<MessageData> Messages
    {
        get => _messages;
    }

    [Server]
    public void PostMessage(string sender, string content, DateTime timestamp)
    {
        _messages.Add(new MessageData(sender, content, timestamp));
    }

    
}
