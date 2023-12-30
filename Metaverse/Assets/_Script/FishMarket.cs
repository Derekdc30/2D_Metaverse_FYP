using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.SceneManagement;
using FishNet.Object;
using TMPro;
using UnityEngine.UI;

public class FishMarket : NetworkBehaviour
{
    public GameObject FishUI;
    public override void OnStartClient(){
        base.OnStartClient();
        if(!base.IsOwner){
            enabled = false;
            return;
        }

        if(FishUI.activeSelf){
            ToggleInventory();
        }
    }    

    public void ToggleInventory(){
        if(FishUI.activeSelf){
            FishUI.SetActive(false);
        }
        else if(!FishUI.activeSelf){
            FishUI.SetActive(true);
        }
    }
}
