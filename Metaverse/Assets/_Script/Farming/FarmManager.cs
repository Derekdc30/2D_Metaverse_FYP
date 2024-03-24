using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine; 
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using FishNet;
using FishNet.Object;

public class FarmManager : NetworkBehaviour
{
    private int availableSlots = 0;
    public const int totalPages = 5;
    public const int slotsPerPage = 6;
    public int totalSlots => totalPages * slotsPerPage;
    private int currentSlotPrice = 5;
    private const int maxSlotPrice = 20480;
    public FarmingItem BuyslotIcon;
    public FarmingItem PlaceHolder;
    public GameObject SeedListObject;
    public GameObject[] Pages = new GameObject[5];
    public List<PlayerInventory.InventoryObject> invetoryobj = new List<PlayerInventory.InventoryObject>();
    public List<FarmingObject> farmingObjects = new List<FarmingObject>();
    [System.Serializable]
    public class FarmingObject{
        public FarmingItem item;
        public string startDate;
        public int stage;
        public int slotnum;
    }
    public GameObject UI;
    private bool hasRunFunction = false;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        PlayerInventory playerInventory = playerObject.GetComponent<PlayerInventory>();
        invetoryobj = playerInventory.getBackpack();
        UI.SetActive(false);
    }
    public override void OnStartClient(){
        base.OnStartClient();
        if(!base.IsOwner){
            enabled = false;
            return;
        } 
    } 
    // Update is called once per frame
    void Update()
    {
        if (UI.activeSelf && !hasRunFunction)
        {
            InitFarming();
            hasRunFunction = true;
        }
        else if(!UI.activeSelf){
            hasRunFunction = false;
        }
    }
    public void BuySlot(){
        availableSlots+=1;
        farmingObjects.RemoveAt(farmingObjects.Count-1);
        farmingObjects.Add(new FarmingObject(){item=PlaceHolder,startDate="",stage=0,slotnum=availableSlots});
        farmingObjects.Add(new FarmingObject(){item=BuyslotIcon,startDate="",stage=0,slotnum=availableSlots+1});
        InitFarming();
    }
    public void AddNewPlant(){
        InitFarming();
        GameObject.Find("Farming_UI/Background/SeedListCanvas").SetActive(true);
        foreach(PlayerInventory.InventoryObject item in invetoryobj){
            Debug.Log(item.item.itemName);
            if(item.item.itemName.Contains("Seed")){
                Transform Holder = GameObject.FindWithTag("SeedListHolder").transform;
                GameObject obj = Instantiate(SeedListObject, Holder);
                TextMeshProUGUI[] Texts = obj.GetComponents<TextMeshProUGUI>();
                Texts[0].text = item.item.itemName;
                Texts[1].text = item.amount.ToString();
            }
        }
    }
    public void InitFarming(){
        for(int i=0; i<5;i++){
            Debug.Log("Destroy");
            foreach (Transform child in Pages[i].transform.GetChild(0))
            {
                Destroy(child.gameObject);
            }
        }
        
        int page=0;
        int slot=1;
        if(availableSlots==0){
            Debug.Log("empty");
            farmingObjects.Add(new FarmingObject(){item=BuyslotIcon,startDate="",stage=0,slotnum=availableSlots+1});
        }
        foreach(FarmingObject farmobj in farmingObjects){
            Debug.Log("Instan");
            GameObject obj = Instantiate(farmobj.item.prefab,Pages[page].transform.GetChild(0));
            if(farmobj.item.itemName == "BuySlot"){
                obj.GetComponent<Button>().onClick.AddListener(()=>{
                    BuySlot();
                });
            }
            else if(farmobj.item.itemName == "PlaceHolder")
            {
                AddNewPlant();
            }
            else{
                //AddNewPlant(farmobj);
                Debug.Log("print");
            }
            slot+=1;
            if(slot >= 7){
                slot=1;
                page+=1;
            }
        }
    }
}