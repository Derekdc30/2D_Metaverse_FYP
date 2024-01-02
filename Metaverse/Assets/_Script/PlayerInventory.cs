using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.SceneManagement;
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
    [SerializeField] KeyCode inventoryButton = KeyCode.E;
    [Header("Pickup setting")]
    [SerializeField] LayerMask pickupLayer;
    [SerializeField] float pickupDistance;
    [SerializeField] KeyCode pickupButton = KeyCode.F;
    Transform worldObjectHolder;
    private InventoryObject selectedInventoryObject;
    private InventoryObject selectedToolBarObject;
    public override void OnStartClient(){
        base.OnStartClient();
        if(!base.IsOwner){
            enabled = false;
            return;
        }
        worldObjectHolder = GameObject.FindWithTag("WorldObjects").transform;
        inventoryPanel = GameObject.FindWithTag("Inventory");
        inventoryHolder = GameObject.FindWithTag("InventoryHolder").transform;
        inventoryPanel.transform.GetChild(4).GetComponent<Button>().gameObject.SetActive(false);
        GameObject.Find("InventoryCanvas/Inventory/Equip").GetComponent<Button>().onClick.AddListener(ToolBar);
        inventoryPanel.SetActive(false);
    }    
    private void Update(){
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
    void AddToInventory(Item newItem){
        Debug.Log("add");
        foreach(InventoryObject invObj in inventoryObjects){
            if(invObj.item == newItem){
                invObj.amount++;
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
    void ToolBar(){
        if(GameObject.FindWithTag("ToolBar")){
            Transform ToolBarHolder = GameObject.FindWithTag("ToolBarHolder").transform;

            foreach(InventoryObject invObj in toolBarObjects){
                if(invObj.item == selectedInventoryObject.item || toolBarObjects.Count>9){
                    return;
                }
            }
            toolBarObjects.Add(new InventoryObject(){item = selectedInventoryObject.item, amount = 1,itemImage=selectedInventoryObject.item.itemImage});

            GameObject obj = Instantiate(toolBarObject, ToolBarHolder);
            Image slotImage = obj.GetComponent<Image>();
            slotImage.sprite = selectedInventoryObject.item.itemImage;
            RectTransform rectTransform = obj.GetComponent<RectTransform>();
            rectTransform.localScale = new Vector3(0.2f, 2f, 1f);
            Debug.Log(toolBarObjects.Count);
            foreach(InventoryObject invObj in toolBarObjects){
                obj.GetComponent<Button>().onClick.AddListener(()=>{
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
    [System.Serializable]
    public class InventoryObject{
        public Item item;
        public int amount;
        public Sprite itemImage;
    }
}
