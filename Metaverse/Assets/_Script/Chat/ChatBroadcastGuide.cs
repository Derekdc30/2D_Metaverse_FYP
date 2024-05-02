using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FishNet;
using FishNet.Broadcast;
using FishNet.Connection;
using UnityEngine.UI;

public class ChatBroadcastGuide : MonoBehaviour
{
    public Transform chatHolder;
    public GameObject msgElement;
    public TMP_InputField playerUsername, playerMessage;

    private void OnEnable()
    {
        InstanceFinder.ClientManager.RegisterBroadcast<Message>(OnMessageReceived);
        InstanceFinder.ServerManager.RegisterBroadcast<Message>(OnClientMessageReceived);
    }

    private void OnDisable()
    {
        InstanceFinder.ClientManager.UnregisterBroadcast<Message>(OnMessageReceived);
        InstanceFinder.ServerManager.UnregisterBroadcast<Message>(OnClientMessageReceived);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            SendChatMessage();
        }
    }    

    public void SendChatMessage()
    {
        if (string.IsNullOrEmpty(playerUsername.text) || string.IsNullOrEmpty(playerMessage.text))
        {
            Debug.LogError("Username or message is empty.");
            return;
        }

        Message msg = new Message()
        {
            username = playerUsername.text,
            message = playerMessage.text
        };

        playerMessage.text = "";

        if (InstanceFinder.IsServer)
            InstanceFinder.ServerManager.Broadcast(msg);
        else if (InstanceFinder.IsClient)
            InstanceFinder.ClientManager.Broadcast(msg);
    }

private void OnMessageReceived(Message msg)
{
    if (msgElement == null)
    {
        Debug.LogError("msgElement prefab is not assigned.");
        return;
    }
    if (chatHolder == null)
    {
        Debug.LogError("chatHolder is not assigned.");
        return;
    }

    GameObject finalMessage = Instantiate(msgElement, chatHolder);
    TextMeshProUGUI textComponent = finalMessage.GetComponent<TextMeshProUGUI>();
    if (textComponent != null)
    {
        textComponent.text = "Username" + ": " + msg.message;
    }
    else
    {
        Debug.LogError("TextMeshProUGUI component not found on the instantiated msgElement.");
    }
}


    private void OnClientMessageReceived(NetworkConnection networkConnection,Message msg)
    {
        InstanceFinder.ServerManager.Broadcast(msg);
    }

    public struct Message : IBroadcast
    {
        public string username;
        public string message;
    }

}
