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
    GameObject inventoryPanel;
    Transform inventoryHolder;
    [SerializeField] GameObject inventoryObject;
    [SerializeField] KeyCode inventoryButton = KeyCode.E;
    [Header("Pickup setting")]
    [SerializeField] LayerMask pickupLayer;
    [SerializeField] float pickupDistance;
    [SerializeField] KeyCode pickupButton = KeyCode.F;
    Transform worldObjectHolder;
    public override void OnStartClient(){
        base.OnStartClient();
        if(!base.IsOwner){
            enabled = false;
            return;
        }
        worldObjectHolder = GameObject.FindWithTag("WorldObjects").transform;
        inventoryPanel = GameObject.FindWithTag("Inventory");
        inventoryHolder = GameObject.FindWithTag("InventoryHolder").transform;

        inventoryPanel.SetActive(false);
    }    
    private void Update(){
        if(Input.GetKeyDown(pickupButton)){ Debug.Log("Press F"); Pickup();}
        if(Input.GetKeyDown(inventoryButton)) { Debug.Log("Press E"); ToggleInventory();}
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
                // If you only want to pick up one item in any direction, you can break out of the loop here
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
        Debug.Log("Toggle inv");
        if(inventoryPanel.activeSelf){
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
        }
    }
    [System.Serializable]
    public class InventoryObject{
        public Item item;
        public int amount;
        public Sprite itemImage;
    }
}
