using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    private DontDestroy instance;

    private void Awake()
    {
        // Check if an instance already exists
        if (instance == null)
        {
            // If no instance exists, make this the singleton instance
            instance = this;
            DontDestroyOnLoad(transform.gameObject);
        }
        else if (instance != this)
        {
            // If an instance already exists and it's not this one, destroy this duplicate
            Destroy(gameObject);
        }
    }
}
