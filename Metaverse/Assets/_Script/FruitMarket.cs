using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.SceneManagement;
using FishNet.Object;
using TMPro;
using UnityEngine.UI;

public class FruitMarket : NetworkBehaviour
{
    public GameObject FruitUI;
    public override void OnStartClient(){
        base.OnStartClient();
        if(!base.IsOwner){
            enabled = false;
            return;
        }

        if(FruitUI.activeSelf){
            ToggleInventory();
        }
    }    

    public void ToggleInventory(){
        if(FruitUI.activeSelf){
            FruitUI.SetActive(false);
        }
        else if(!FruitUI.activeSelf){
            FruitUI.SetActive(true);
        }
    }
}
