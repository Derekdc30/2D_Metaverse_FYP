using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using FishNet.Object;
using TMPro;
using UnityEngine.UI;
public class OpenUI : NetworkBehaviour
{
    public GameObject UI;
    private NetworkObject nob;
    public override void OnStartClient(){
        base.OnStartClient();
        if(!base.IsOwner){
            enabled = false;
            return;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
       nob = other.GetComponent<NetworkObject>();
        if(nob != null){
            UI.SetActive(true);
        }
    }
}
