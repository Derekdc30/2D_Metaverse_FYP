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

    public Item[] MarketItem;
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

    public void Buy(string value){
        string name = value.Split(',')[0];
        string price = value.Split(',')[1];
        string UserName = PlayerPrefs.GetString("name");
        Economic economic = GetComponent<Economic>();
        if(int.Parse(GameObject.FindWithTag("MoneyText").GetComponent<TextMeshProUGUI>().text.Substring(1)) >= int.Parse(price)){
            economic.SyncMoneyroutine(price,"2",PlayerPrefs.GetString("name"));
            GameObject.Find("VegetableSellUI/Background/message").SetActive(true);
            GameObject.Find("VegetableSellUI/Background/message").GetComponentInChildren<TextMeshProUGUI>().text = "Successfully buy "+name;
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            PlayerInventory playerInventory = playerObject.GetComponent<PlayerInventory>();
            foreach(Item item in MarketItem){
                if(item.itemName == name){
                    playerInventory.AddToInventory(item);
                    playerInventory.SyncInventoryroutine(item.itemName.ToString(),"1","1",UserName);
                }
            }
        }
        else{
            GameObject.Find("VegetableSellUI/Background/message").SetActive(true);
            GameObject.Find("VegetableSellUI/Background/message").GetComponentInChildren<TextMeshProUGUI>().text = "Not enough money";
        }
        economic.SyncMoneyroutine("0","3",UserName);
    } 
}