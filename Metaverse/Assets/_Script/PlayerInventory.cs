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
        foreach(InventoryObject invObj in inventoryObjects){
            if(invObj.item == newItem){
                invObj.amount++;
                return;
            }
        }
        inventoryObjects.Add(new InventoryObject(){item = newItem, amount = 1});
    }
    public void ToggleInventory(){
        if(inventoryPanel.activeSelf){
            Debug.Log("Press E");
            inventoryPanel.SetActive(false);
            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
        }
        else if(!inventoryPanel.activeSelf){
            inventoryPanel.SetActive(true);
            //Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;
        }
    }
    /*void UpdateInvUI()
    {
        foreach (Transform child in invObjectHolder)
            Destroy(child.gameObject);

        foreach (InventoryObject invObj in inventoryObjects)
        {
            GameObject obj = Instantiate(invCanvasObject, invObjectHolder);

            // Assuming that the invCanvasObject has an Image component at index 1 (change as needed)
            Image itemImage = obj.transform.GetChild(1).GetComponent<Image>();

            // Load and assign the sprite to the Image component
            if (invObj.item.itemImage != null) // Assuming itemImage is a reference to the sprite
                itemImage.sprite = invObj.item.itemImage;

            // Assuming you want to display the amount as text
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = invObj.amount.ToString();

            obj.GetComponent<Button>().onClick.AddListener(delegate { DropItem(invObj.item); });
        }
    }*/

    [System.Serializable]
    public class InventoryObject{
        public Item item;
        public int amount;
    }
}
