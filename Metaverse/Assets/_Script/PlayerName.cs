using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class PlayerName : NetworkBehaviour
{
    private ChatManager _chat; //For Chatting
    private string _messageContent = string.Empty; //For Chatting
    [SerializeField] public TMP_Text usernameText;
    [SyncVar(OnChange = nameof(SetupNameOnClient))]
    string playerName;
    public delegate void PlayerSetupName(string name);
    public static PlayerSetupName playerSetupName;
    public override void OnStartNetwork(){
        playerName = PlayerPrefs.GetString("name");
        //Chatting Function
        _chat = FindAnyObjectByType<ChatManager>(FindObjectsInactive.Include);
    }
    private void OnEnable() {
        playerSetupName += SetupNameOnServerRpc;
    }
    private void OnDisable() {
        playerSetupName -= SetupNameOnServerRpc;
    }
    void SetupNameOnClient(string oldName, string newName, bool asServer){
        Debug.Log("username: " + newName + " " + oldName);
        usernameText.text = newName;
    }
    [ServerRpc]
    void SetupNameOnServerRpc(string name){
        playerName = name;
    }
    public void SetupName(){
        playerSetupName?.Invoke(PlayerPrefs.GetString("name"));
    }

    // For Chatting
    [ServerRpc]
    private void ServerPostMessage(DateTime timestamp, string content)
    {
        _chat.PostMessage(playerName, content, timestamp);
    }

    /* WIP
    private void OnGUI()
    {
        if (!IsOwner) return;

        GUILayoutUtility.GetRect(Screen.width, Screen.height * 0.125f);
        GUILayout.BeginHorizontal();
        _messageContent = GUILayout.TextField(_messageContent);
        if (GUILayout.Button("Send"))
            ServerPostMessage(DateTime.Now, _messageContent);
        GUILayout.EndHorizontal();
        for (int i = 0; i < _chat.Messages.Count; i++)
        {
            MessageData message = _chat.Messages[i];

            GUILayout.BeginHorizontal();
            GUILayout.Label($"{message.Timestamp} {message.Sender}: {message.Content}");

            GUILayout.EndHorizontal();
        }
    }
    */
}
