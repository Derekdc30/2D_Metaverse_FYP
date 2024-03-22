using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using FishNet.Object;
using TMPro;
using UnityEngine.UI;


public class PlayerInventory : NetworkBehaviour
{
    [Header("Inventory settings")]
    public List<InventoryObject> inventoryObjects = new List<InventoryObject>();
    public List<InventoryObject> toolBarObjects = new List<InventoryObject>();
    GameObject inventoryPanel;
    Transform inventoryHolder;
    [SerializeField] GameObject inventoryObject;
    [SerializeField] GameObject toolBarObject;
    [SerializeField] KeyCode inventoryButton = KeyCode.Tab;
    [Header("Pickup setting")]
    [SerializeField] LayerMask pickupLayer;
    [SerializeField] float pickupDistance;
    [SerializeField] KeyCode pickupButton = KeyCode.F;
    Transform worldObjectHolder;
    private InventoryObject selectedInventoryObject;
    private InventoryObject selectedToolBarObject;
    [Header("Sync")]
    public Item[] Itemlist;
    private string[] Items = new string[0];
    private string[] Value = new string[0];
    [SerializeField] string InventoryURL = "http://127.0.0.1:3000/user/Inventory";
    public class InventoryData{
        public string Items;
        public string Value;
    }
    private Economic economic; 
    public override void OnStartClient(){
        base.OnStartClient();
        if(!base.IsOwner){
            enabled = false;
            return;
        }
        economic = GetComponent<Economic>();
        economic.SyncMoneyroutine("0","3",PlayerPrefs.GetString("name"));
        worldObjectHolder = GameObject.FindWithTag("WorldObjects").transform;
        inventoryPanel = GameObject.FindWithTag("Inventory");
        inventoryHolder = GameObject.FindWithTag("InventoryHolder").transform;
        inventoryPanel.transform.GetChild(4).GetComponent<Button>().gameObject.SetActive(false);
        GameObject.Find("InventoryCanvas/Inventory/Equip").GetComponent<Button>().onClick.AddListener(ToolBar);
        inventoryPanel.SetActive(false);
        SyncInventoryroutine("","","3",PlayerPrefs.GetString("name"));
    }    
    private void Update()
    {
        ProcessInput();
        if(Input.GetKeyDown(pickupButton)){ Pickup();}
        if(Input.GetKeyDown(inventoryButton)) { ToggleInventory();}
    }
    void Pickup()
    {
        // Define directions to check (up, down, left, right)
        Vector2[] directions = { transform.up, -transform.up, -transform.right, transform.right };

        foreach (Vector2 direction in directions)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, pickupDistance, pickupLayer);

            if (hit.collider != null)
            {
                GroundItem groundItem = hit.collider.GetComponent<GroundItem>();
                if (groundItem != null)
                {
                    AddToInventory(groundItem.itemScriptable);
                    DespawnObject(groundItem.gameObject);
                    break;
                }
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    void DespawnObject(GameObject objToDespawn){
        ServerManager.Despawn(objToDespawn,DespawnType.Destroy);
    }
    public void AddToInventory(Item newItem){
        foreach(InventoryObject invObj in inventoryObjects){
            if(invObj.item == newItem){
                invObj.amount++;
                return;
            }
        }
        inventoryObjects.Add(new InventoryObject(){item = newItem, amount = 1,itemImage=newItem.itemImage});
    }
    public void AddManyToInventory(Item newItem, int amount){
        foreach(InventoryObject invObj in inventoryObjects){
            if(invObj.item == newItem){
                invObj.amount= amount;
                return;
            }
        }
        inventoryObjects.Add(new InventoryObject(){item = newItem, amount = 1,itemImage=newItem.itemImage});
    }
    public void ToggleInventory(){
        if(inventoryPanel.activeSelf){
            inventoryPanel.transform.GetChild(4).GetComponent<Button>().gameObject.SetActive(false);
            inventoryPanel.SetActive(false);
        }
        else if(!inventoryPanel.activeSelf){
            SyncInventoryroutine("","","3",PlayerPrefs.GetString("name"));
            UpdateInvUI();
            inventoryPanel.SetActive(true);
        }
    }
    void UpdateInvUI()
    {
        foreach (Transform child in inventoryHolder)
            Destroy(child.gameObject);

        foreach (InventoryObject invObj in inventoryObjects)
        {
            GameObject obj = Instantiate(inventoryObject, inventoryHolder);
            Image slotImage = obj.GetComponent<Image>();
            TextMeshProUGUI itemCountText = obj.GetComponentInChildren<TextMeshProUGUI>();
            slotImage.sprite = invObj.item.itemImage;
            itemCountText.text = invObj.amount.ToString();
            obj.GetComponent<Button>().onClick.AddListener(()=>{
                GameObject.Find("InventoryCanvas/Inventory/Item_Description").SetActive(true);
                GameObject.Find("InventoryCanvas/Inventory/Item_Description").GetComponent<TextMeshProUGUI>().text = invObj.item.itemName;
                ActiveEquipButton();
                SelectInventoryObject(invObj);
                });
        }
    }
    void ActiveEquipButton(){
        if(GameObject.FindWithTag("ToolBar")){
            GameObject.Find("InventoryCanvas/Inventory/Equip").GetComponent<Button>().gameObject.SetActive(true);
        }
    }
    void SelectInventoryObject(InventoryObject selectedObject)
    {
        selectedInventoryObject = selectedObject;
    }
    void SelectToolBarObject(InventoryObject selectedObject)
    {
        selectedToolBarObject = selectedObject;
    }
    void ToolBar()
    {
        if (GameObject.FindWithTag("ToolBar"))
        {
            Transform ToolBarHolder = GameObject.FindWithTag("ToolBarHolder").transform;
            bool itemInToolBar = toolBarObjects.Exists(obj => obj.item == selectedInventoryObject.item);

            // Check if there is space in the toolbar (less than 9 items)
            if (toolBarObjects.Count < 9)
            {
                // If there is space, add the selected item at the end
                toolBarObjects.Add(new InventoryObject() { item = selectedInventoryObject.item, amount = 1, itemImage = selectedInventoryObject.item.itemImage });
            }
            else
            {
                if (itemInToolBar)
                {
                    int index = toolBarObjects.FindIndex(obj => obj.item == selectedInventoryObject.item);
                    toolBarObjects[index] = new InventoryObject() { item = selectedInventoryObject.item, amount = 1, itemImage = selectedInventoryObject.item.itemImage };
                    return;
                }
                int selectedIndex = toolBarObjects.FindIndex(obj => obj.item == selectedToolBarObject.item);
                if (selectedIndex != -1)
                {
                    toolBarObjects[selectedIndex] = new InventoryObject() { item = selectedInventoryObject.item, amount = 1, itemImage = selectedInventoryObject.item.itemImage };
                }
            }
            UpdateToolBarUI();
        }
    }

    void UpdateToolBarUI()
    {
        if (GameObject.FindWithTag("ToolBar"))
        {
            Transform ToolBarHolder = GameObject.FindWithTag("ToolBarHolder").transform;

            // Destroy existing toolbar items
            foreach (Transform child in ToolBarHolder)
            {
                Destroy(child.gameObject);
            }

            // Instantiate new toolbar items
            foreach (InventoryObject invObj in toolBarObjects)
            {
                GameObject obj = Instantiate(toolBarObject, ToolBarHolder);
                Image slotImage = obj.GetComponent<Image>();
                slotImage.sprite = invObj.item.itemImage;
                RectTransform rectTransform = obj.GetComponent<RectTransform>();
                rectTransform.localScale = new Vector3(0.2f, 1.8f, 1f);
                GameObject indicator = obj.transform.Find("Highlight").gameObject;
                indicator.SetActive(invObj == selectedToolBarObject);
                // Add a click listener to handle selection
                obj.GetComponent<Button>().onClick.AddListener(() => {
                    foreach (Transform child in ToolBarHolder)
                    {
                        child.transform.Find("Highlight").gameObject.SetActive(false);
                    }
                    indicator.SetActive(true);
                    SelectToolBarObject(invObj);
                    ToolBarSelect();
                });
            }
        }
    }
    void ToolBarSelect(){
        if(GameObject.FindWithTag("ToolBar")){
            GameObject.Find("ToolBar/Background/ToolName").GetComponent<TextMeshProUGUI>().text = selectedToolBarObject.item.itemName;
        }
    }
    void ProcessInput()
    {
        if (!GameObject.FindWithTag("ToolBar")){
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)) { SelectToolBarItem(0); }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) { SelectToolBarItem(1); }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) { SelectToolBarItem(2); }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) { SelectToolBarItem(3); }
        else if (Input.GetKeyDown(KeyCode.Alpha5)) { SelectToolBarItem(4); }
        else if (Input.GetKeyDown(KeyCode.Alpha6)) { SelectToolBarItem(5); }
        else if (Input.GetKeyDown(KeyCode.Alpha7)) { SelectToolBarItem(6); }
        else if (Input.GetKeyDown(KeyCode.Alpha8)) { SelectToolBarItem(7); }
        else if (Input.GetKeyDown(KeyCode.Alpha9)) { SelectToolBarItem(8); }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            // Use the mouse scroll to select the next or previous toolbar item
            int currentIndex = toolBarObjects.IndexOf(selectedToolBarObject);
            int newIndex = (currentIndex + (scroll > 0f ? 1 : -1)) % toolBarObjects.Count;
            if (newIndex < 0)
            {
                newIndex = toolBarObjects.Count - 1;
            }
            SelectToolBarItem(newIndex);
        }
    }
    public List<InventoryObject> getBackpack(){
        return inventoryObjects;
    }

    void SelectToolBarItem(int index)
    {
        if (index >= 0 && index < toolBarObjects.Count)
        {
            // Trigger the click event for the toolbar item at the specified index
            Transform ToolBarHolder = GameObject.FindWithTag("ToolBarHolder").transform;
            Button toolbarButton = ToolBarHolder.GetChild(index).GetComponent<Button>();
            toolbarButton.onClick.Invoke();
        }
    }
    public void SyncInventoryroutine(string item, string value, string mode, string username){
        StartCoroutine(SyncInventory(item,value,mode,username));
    }
    IEnumerator SyncInventory(string item, string value, string mode, string username){
        WWWForm form = new WWWForm();
        form.AddField("mode", mode);
        form.AddField("userName",username);
        form.AddField("item", item);
        form.AddField("value",value);
        using(UnityWebRequest www = UnityWebRequest.Post(InventoryURL,form)){
            yield return www.SendWebRequest();
            if(www.result == UnityWebRequest.Result.ConnectionError){
                Debug.Log(www.error);
            }
            else{
                Debug.Log(www.downloadHandler.text);
                if(www.responseCode == 200){
                    var responseData = JsonUtility.FromJson<InventoryData>(www.downloadHandler.text);
                    Items = responseData.Items.Split(",");
                    Value = responseData.Value.Split(",");
                    if(mode == "3"){
                        for(int i = 0; i<Items.Length;i++){
                            foreach(Item tempitem in Itemlist){
                                if(tempitem.itemName == Items[i]){
                                    AddManyToInventory(tempitem,int.Parse(Value[i]));
                                }
                            }
                        }
                    }
                }
                else{
                    Debug.Log("Error"+ www.downloadHandler.text);
                }
            }
        }
    }
    [System.Serializable]
    public class InventoryObject{
        public Item item;
        public int amount;
        public Sprite itemImage;
    }
}
