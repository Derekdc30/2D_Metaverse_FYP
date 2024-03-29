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
    public GameObject MessageBox;
    public GameObject[] Pages = new GameObject[5];
    public List<FarmingItem> SeedDB = new List<FarmingItem>();
    public List<Item> ItemDB = new List<Item>();
    private List<PlayerInventory.InventoryObject> invetoryobj;
    public List<FarmingObject> farmingObjects = new List<FarmingObject>();
    [System.Serializable]
    public class FarmingObject{
        public FarmingItem item;
        public DateTime startDate;
        public int stage;
        public int slotnum;
    }
    public GameObject UI;
    private bool hasRunFunction = false;
    private bool isGrowing = false;

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
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        Economic economic = playerObject.GetComponent<Economic>();
        if(int.Parse(GameObject.FindWithTag("MoneyText").GetComponent<TextMeshProUGUI>().text.Substring(1))>= BuyslotIcon.price){
            farmingObjects.RemoveAt(farmingObjects.Count-1);
            farmingObjects.Add(new FarmingObject(){item=PlaceHolder,startDate=DateTime.MinValue,stage=0,slotnum=availableSlots});
            farmingObjects.Add(new FarmingObject(){item=BuyslotIcon,startDate=DateTime.MinValue,stage=0,slotnum=availableSlots+1});
            availableSlots+=1;
            if(BuyslotIcon.price==0){
                BuyslotIcon.price = 5;
            }else{
                BuyslotIcon.price = BuyslotIcon.price*2;
            }
            economic.SyncMoneyroutine(BuyslotIcon.price.ToString(),"2",PlayerPrefs.GetString("name"));
        }
        InitFarming();
    }
    public void AddNewPlant(FarmingObject farmobj){
        GameObject.Find("Farming_UI/Background/SeedListCanvas").SetActive(true);
        foreach (Transform child in GameObject.FindWithTag("SeedListHolder").transform)        // clear current item
                Destroy(child.gameObject); 
        foreach(PlayerInventory.InventoryObject item in invetoryobj){   
            if(item.item.itemName.Contains("Seed") && item.amount>0){
                Transform Holder = GameObject.FindWithTag("SeedListHolder").transform;
                GameObject obj = Instantiate(SeedListObject, Holder);
                TextMeshProUGUI[] Texts = obj.GetComponentsInChildren<TextMeshProUGUI>();
                Texts[0].text = item.item.itemName;
                Texts[1].text = item.amount.ToString();
                obj.GetComponent<Button>().onClick.AddListener(()=>{
                    GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                    PlayerInventory playerInventory = playerObject.GetComponent<PlayerInventory>();
                    playerInventory.RemoveOne(item.item);
                     playerInventory.SyncInventoryroutine(item.itemName.ToString(),"1","2",PlayerPrefs.GetString("name"));
                    AddNewSeed(farmobj,item);
                    GameObject.Find("Farming_UI/Background/SeedListCanvas").SetActive(false);
                });
            }
        }
    }
    public void AddNewSeed(FarmingObject obj, PlayerInventory.InventoryObject item){
        foreach(FarmingObject fobj in farmingObjects){
            if(obj.slotnum == fobj.slotnum){
                foreach(FarmingItem seed in SeedDB){
                    if(seed.itemName == item.item.itemName){
                        fobj.item = seed;
                        fobj.startDate = DateTime.Now;
                    }
                }
               
            }
        }
        StartGrowing();
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
            farmingObjects.Add(new FarmingObject(){item=BuyslotIcon,startDate=DateTime.MinValue,stage=0,slotnum=availableSlots+1});
            availableSlots +=1;
        }
        foreach(FarmingObject farmobj in farmingObjects){
            FarmingObject currentFarmObj = farmobj; // Create a temporary variable
            GameObject obj = Instantiate(currentFarmObj.item.prefab,Pages[page].transform.GetChild(0));
            if(farmobj.item.itemName.Contains("Seed")){
                Debug.Log("Seed only");
                TextMeshProUGUI Texts = obj.GetComponentInChildren<TextMeshProUGUI>();
                Texts.text = farmobj.item.itemName;
                Image slotImage = obj.GetComponent<Image>();
                slotImage.sprite = farmobj.item.stages[farmobj.stage];
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
                else if(currentFarmObj.item.itemName.Contains("Seed")){
                    HarvestPlant(farmobj);
                }
                else{
                    Debug.Log("ERROR");
                }
            });
            slot+=1;
            if(slot >= 7){
                slot=1;
                page+=1;
            }
        }
    }
    private void StartGrowing()
    {
        if (!isGrowing)
        {
            isGrowing = true;
            InvokeRepeating("CheckHarvestStatus", 60f, 60f); // Check every 10 seconds for testing
        }
        
    }
    private void CheckHarvestStatus()
    {
        DateTime currentTime = DateTime.Now;
        foreach(FarmingObject fobj in farmingObjects){
            if(fobj.item.itemName.Contains("Seed")){
                TimeSpan growthDuration = TimeSpan.FromMinutes(fobj.item.duration);
                DateTime harvestTime = fobj.startDate + growthDuration;
                if (currentTime >= harvestTime)
                {
                    Debug.Log("Seed is ready to harvest!");
                }
                else
                {
                    if(fobj.stage<=3){
                        fobj.stage++;
                    }
                    
                    Debug.Log("Seed is still growing");
                }
            }
        }
        InitFarming();
    } 
    private void HarvestPlant(FarmingObject fobj){
        if(fobj.stage>=1){
            foreach(Item item in ItemDB){
                if(item.itemName == fobj.item.itemName){
                    string[] words = fobj.item.itemName.Split(' ');
                    MessageBox.SetActive(true);
                    TextMeshProUGUI texts = MessageBox.GetComponentInChildren<TextMeshProUGUI>();
                    texts.text = "You earn $" + fobj.item.price+" from "+words[0];
                    GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                    Economic economic = playerObject.GetComponent<Economic>();
                    economic.SyncMoneyroutine(fobj.item.price,"1",PlayerPrefs.GetString("name"));
                }
            }
            foreach(FarmingObject obj in farmingObjects){
                if(obj.slotnum == fobj.slotnum){
                    obj.item = PlaceHolder;
                    obj.startDate =DateTime.MinValue;
                }
            }
        }
        else{
            return;
        }
        InitFarming();
    }
}
