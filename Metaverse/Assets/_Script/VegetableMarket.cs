using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.SceneManagement;
using FishNet.Object;
using TMPro;
using UnityEngine.UI;

public class VegetableMarket : NetworkBehaviour
{
    public GameObject VegeUI;
    public override void OnStartClient(){
        base.OnStartClient();
        if(!base.IsOwner){
            enabled = false;
            return;
        }

        if(VegeUI.activeSelf){
            ToggleInventory();
        }
    }    

    public void ToggleInventory(){
        if(VegeUI.activeSelf){
            VegeUI.SetActive(false);
        }
        else if(!VegeUI.activeSelf){
            VegeUI.SetActive(true);
        }
    }
}
