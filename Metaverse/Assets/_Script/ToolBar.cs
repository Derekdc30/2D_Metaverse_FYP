using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using TMPro;
using UnityEngine.UI;

public class ToolBar : MonoBehaviour
{
    //deprecated
    /*[Header("Inventory settings")]
    public List<InventoryObject> inventoryObjects = new List<InventoryObject>();
    public List<Image> toolbarSlots = new List<Image>(); // References to the toolbar slots
    public KeyCode[] toolbarKeys = {
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5,
        KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9
    };
    private void Update()
    {
        if (Input.GetKeyDown(pickupButton)) { Pickup(); }
        if (Input.GetKeyDown(inventoryButton)) { ToggleInventory(); }
        UpdateToolbarInput();
    }
    void UpdateToolbarInput()
    {
        for (int i = 0; i < toolbarKeys.Length; i++)
        {
            if (Input.GetKeyDown(toolbarKeys[i]))
            {
                if (i < inventoryObjects.Count)
                {
                    SwitchToItem(i);
                }
            }
        }
    }

    void SwitchToItem(int slotIndex)
    {
        InventoryObject selectedSlot = inventoryObjects[slotIndex];

        Debug.Log($"Switched to item in toolbar slot {slotIndex + 1}");
    }*/
}
