using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    private static DontDestroy instance;
    public List<GameObject> objectsToPersist = new List<GameObject>();

    private void Awake()
    {
        // Ensure only one instance of the manager exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Apply DontDestroyOnLoad to each object in the list
            foreach (GameObject obj in objectsToPersist)
            {
                DontDestroyOnLoad(obj);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
