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
    [SerializeField] private string FarmingURL = "http://127.0.0.1:3000/user/Farming";
    private int availableSlots = 0;
    public const int totalPages = 5;
    public const int slotsPerPage = 6;
    public int totalSlots => totalPages * slotsPerPage;
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
    [System.Serializable]
    public class FarmingData
    {
        public string UserName;
        public int available;
        public FarmingObjectData[] farmingObjects;
    }
    [System.Serializable]
    public class FarmingObjectData
    {
        public string item;
        public string startDate;
        public int stage;
        public int slotnum;
    }
    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
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
            StartCoroutine(FarmingHandler(UserName:PlayerPrefs.GetString("name"),mode:"1",available:0,item:"",startDate:"",stage:0,slotnum:0));
            //InitFarming();
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
                    playerInventory.SyncInventoryroutine(item.item.itemName.ToString(),"1","2",PlayerPrefs.GetString("name"));
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
                    fobj.stage=4;
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
                    economic.SyncMoneyroutine(fobj.item.price.ToString(),"1",PlayerPrefs.GetString("name"));
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
    public void syncFarming(){
        foreach(FarmingObject fobj in farmingObjects){
            StartCoroutine(FarmingHandler(UserName:PlayerPrefs.GetString("name"),mode:"0",available:availableSlots,item:fobj.item.itemName,startDate:fobj.startDate.ToString(),stage:fobj.stage,slotnum:fobj.slotnum));
        }
    }
    IEnumerator FarmingHandler (string UserName, string mode, int available, string item, string startDate, int stage, int slotnum){
        WWWForm form = new WWWForm();
        form.AddField("UserName",UserName);
        form.AddField("mode",mode);
        form.AddField("available",available);
        form.AddField("item",item);
        form.AddField("startDate",startDate);
        form.AddField("stage",stage);
        form.AddField("slotnum",slotnum);
        using(UnityWebRequest www = UnityWebRequest.Post(FarmingURL, form)){
            yield return www.SendWebRequest();
            if(www.result == UnityWebRequest.Result.ConnectionError){
                Debug.Log(www.error);
            }
            else
            {
                if (www.responseCode == 200) // Check if the request was successful
                {
                    string jsonResponse = www.downloadHandler.text;
                    Debug.Log("Received JSON response: " + jsonResponse);
                    FarmingData farmingData = JsonUtility.FromJson<FarmingData>(jsonResponse);
                    availableSlots = farmingData.available;
                    if(availableSlots>0){
                        farmingObjects = new List<FarmingObject>();
                        foreach(FarmingObjectData data in farmingData.farmingObjects){
                            if(data.item == "PlaceHolder"){
                                farmingObjects.Add(new FarmingObject(){item=PlaceHolder,startDate=DateTime.MinValue,stage=0,slotnum=data.slotnum});
                            }else if(data.item == "BuySlot"){
                                farmingObjects.Add(new FarmingObject(){item=BuyslotIcon,startDate=DateTime.MinValue,stage=0,slotnum=data.slotnum});
                            }else if(data.item.Contains("Seed")){
                                foreach(FarmingItem seed in SeedDB){
                                    if(seed.itemName == data.item){
                                        DateTime temp;
                                        DateTime.TryParse(data.startDate, out temp);
                                        farmingObjects.Add(new FarmingObject(){item = seed, startDate = temp, stage = data.stage, slotnum = data.slotnum});
                                    }
                                }
                            }   
                        }
                    }
                    InitFarming();
                    CheckHarvestStatus();
                    StartGrowing();
                }
                else
                {
                    Debug.Log("Error " + www.responseCode + ": " + www.downloadHandler.text);
                }
            }
        }
    }
}
