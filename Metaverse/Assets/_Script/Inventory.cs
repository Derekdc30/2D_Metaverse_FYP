using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private static Inventory instance;

    private void Awake()
    {
        // Check if an instance already exists
        if (instance == null)
        {
            // If no instance exists, make this the singleton instance
            instance = this;
            DontDestroyOnLoad(transform.gameObject);
        }
        else
        {
            // If an instance already exists, destroy this duplicate
            Destroy(gameObject);
        }
    }
}