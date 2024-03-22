using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class FarmManager : MonoBehaviour
{
    private int availableSlots = 0;
    public const int totalPages = 5;
    public const int slotsPerPage = 6;
    public int totalSlots => totalPages * slotsPerPage;
    private int currentSlotPrice = 5;
    private const int maxSlotPrice = 20480;
    public FarmingItem BuyslotIcon;
    public FarmingItem PlaceHolder;
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
    void Start()
    {
        InitFarming();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        PlayerInventory playerInventory = playerObject.GetComponent<PlayerInventory>();
        invetoryobj = playerInventory.getBackpack();
    }

    // Update is called once per frame
    void Update()
    {
        /*int page = 0;
        int slot = 0;
        page = (availableSlots - 1) / slotsPerPage + 1;
        slot = (availableSlots - 1) % slotsPerPage + 1;
        Debug.Log("Page: "+farmingObjects.Count);*/
    }
    public void BuySlot(){
        availableSlots+=1;
        farmingObjects.RemoveAt(farmingObjects.Count-1);
        farmingObjects.Add(new FarmingObject(){item=PlaceHolder,startDate="",stage=0,slotnum=availableSlots});
        farmingObjects.Add(new FarmingObject(){item=BuyslotIcon,startDate="",stage=0,slotnum=availableSlots+1});
        InitFarming();
    }
    public void AddNewPlant(){

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
        }
        foreach(FarmingObject farmobj in farmingObjects){
            GameObject obj = Instantiate(farmobj.item.prefab,Pages[page].transform.GetChild(0));
            if(farmobj.item.itemName == "BuySlot"){
                obj.GetComponent<Button>().onClick.AddListener(()=>{
                    BuySlot();
                });
            }
            else if(farmobj.item.itemName == "PlaceHolder")
            {
                Debug.Log("placeholder");
            }
            else{
                Debug.Log("plant");
            }
            slot+=1;
            if(slot >= 7){
                slot=1;
                page+=1;
            }
        }
    }
}
