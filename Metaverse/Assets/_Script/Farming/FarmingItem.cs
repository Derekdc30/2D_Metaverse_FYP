using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "FarmingItem", menuName = "Inventory/FarmingItem",order = 2)]
public class FarmingItem : ScriptableObject
{
    public string itemName;
    public int price;
    public int duration;
    public GameObject prefab;
    public Sprite soil;
    public Sprite[] stages = new Sprite[5];

}
