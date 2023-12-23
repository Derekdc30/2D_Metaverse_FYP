using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    Camera cam;
    Transform worldObjectHolder;
    public override void OnStartClient(){
        base.OnStartClient();
        if(!base.IsOwner){
            enabled = false;
            return;
        }
        cam = Camera.main;
        worldObjectHolder = GameObject.FindWithTag("WorldObjects").transform;
        inventoryPanel = GameObject.FindWithTag("Inventory");
        inventoryHolder = GameObject.FindWithTag("InventoryHolder").transform;

        if(inventoryPanel.activeSelf){
            ToggleInventory();
        }
    }
    private void Update(){
        if(Input.GetKeyDown(pickupButton)){ Pickup();}
        if(Input.GetKeyDown(inventoryButton)) { ToggleInventory();}
    }
    void Pickup(){
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, pickupDistance, pickupLayer);

        if (hit.collider != null)
        {
            GroundItem groundItem = hit.collider.GetComponent<GroundItem>();
            if (groundItem != null)
            {
                AddToInventory(groundItem.itemScriptable);
                DespawnObject(groundItem.gameObject);
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
            inventoryPanel.SetActive(false);
            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
        }
        else if(!inventoryPanel.activeSelf){
            UpdateInvUI();
            inventoryPanel.SetActive(true);
            //Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;
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

            //obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = invObj.item.itemName + " - " + invObj.amount;

            /*Image imageComponent = */
            
            /*if (invObj.item.itemImage != null)
            {
                imageComponent = invObj.item.itemImage;
                imageComponent.enabled = true;
            }
            else
            {
                imageComponent.enabled = false;
            }*/
        }
    }
    [System.Serializable]
    public class InventoryObject{
        public Item item;
        public int amount;
        public Sprite itemImage;
    }
}
