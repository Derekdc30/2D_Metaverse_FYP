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
    public List<FarmingItem> SeedDB = new List<FarmingItem>();
    private List<PlayerInventory.InventoryObject> invetoryobj;
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
        farmingObjects.RemoveAt(farmingObjects.Count-1);
        farmingObjects.Add(new FarmingObject(){item=PlaceHolder,startDate="",stage=0,slotnum=availableSlots});
        farmingObjects.Add(new FarmingObject(){item=BuyslotIcon,startDate="",stage=0,slotnum=availableSlots+1});
        availableSlots+=1;
        InitFarming();
    }
    public void AddNewPlant(FarmingObject farmobj){
        GameObject.Find("Farming_UI/Background/SeedListCanvas").SetActive(true);
        foreach (Transform child in GameObject.FindWithTag("SeedListHolder").transform)        // clear current item
                Destroy(child.gameObject); 
        foreach(PlayerInventory.InventoryObject item in invetoryobj){   
            if(item.item.itemName.Contains("Seed")){
                Transform Holder = GameObject.FindWithTag("SeedListHolder").transform;
                GameObject obj = Instantiate(SeedListObject, Holder);
                TextMeshProUGUI[] Texts = obj.GetComponentsInChildren<TextMeshProUGUI>();
                Texts[0].text = item.item.itemName;
                Texts[1].text = item.amount.ToString();
                obj.GetComponent<Button>().onClick.AddListener(()=>{
                    AddNewSeed(farmobj,item);
                    GameObject.Find("Farming_UI/Background/SeedListCanvas").SetActive(false);
                });
            }
        }
    }
    public void AddNewSeed(FarmingObject obj, PlayerInventory.InventoryObject item){
        foreach(FarmingObject fobj in farmingObjects){
            if(fobj.slotnum ==obj.slotnum){
                fobj.item.itemName = item.item.itemName;
                foreach(FarmingItem DB in SeedDB){
                    if(fobj.item.itemName == DB.itemName){
                        fobj.item = DB;
                    }
                }
                fobj.stage = 1;
            }
            Debug.Log(fobj.item.itemName);
        }
        InitFarming();
    }
    public void InitFarming(){
    for(int i=0; i<5;i++){
        foreach (Transform child in Pages[i].transform.GetChild(0))
        {
            Destroy(child.gameObject);
        }
    }
    int page=0;
    int slot=1;
    if(availableSlots==0){
        farmingObjects.Add(new FarmingObject(){item=BuyslotIcon,startDate="",stage=0,slotnum=availableSlots+1});
        availableSlots +=1;
    }
    foreach(FarmingObject farmobj in farmingObjects){
        FarmingObject currentFarmObj = farmobj; // Create a temporary variable
        GameObject obj = Instantiate(currentFarmObj.item.prefab,Pages[page].transform.GetChild(0));
        if(farmobj.item.itemName.Contains("Seed")){
            TextMeshProUGUI Texts = obj.GetComponentInChildren<TextMeshProUGUI>();
            Texts.text = farmobj.item.itemName;
            Image slotImage = obj.GetComponent<Image>();
            slotImage.sprite = farmobj.item.stages[0];
        }
        
        obj.GetComponent<Button>().onClick.RemoveAllListeners();
        obj.GetComponent<Button>().onClick.AddListener(()=>{
            if(currentFarmObj.item.itemName == "BuySlot"){
                BuySlot();
            }
            else if(currentFarmObj.item.itemName == "PlaceHolder")
            {
                AddNewPlant(farmobj);
            }
            else if(currentFarmObj.item.itemName == "Seed"){
                Debug.Log("print");
            }
        });
        slot+=1;
        if(slot >= 7){
            slot=1;
            page+=1;
        }
    }
}
}
