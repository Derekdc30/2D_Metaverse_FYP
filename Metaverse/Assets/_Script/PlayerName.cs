using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerName : NetworkBehaviour
{
    [SerializeField] public TMP_Text usernameText;
    [SyncVar(OnChange = nameof(SetupNameOnClient))]
    string playerName;
    public delegate void PlayerSetupName(string name);
    public static PlayerSetupName playerSetupName;
    public override void OnStartNetwork(){
        playerName = PlayerPrefs.GetString("name");
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
}
