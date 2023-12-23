using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item",order = 1)]
public class Item : ScriptableObject
{
    public string itemName;
    public GameObject prefab;
}
