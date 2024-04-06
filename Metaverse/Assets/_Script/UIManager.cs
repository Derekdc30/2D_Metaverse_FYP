using FishNet;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] uiElementsForClient; //to hold UI GameObjects

    private void Start()
    {
        // Subscribe to the OnClientConnectionState event
        InstanceFinder.ClientManager.OnClientConnectionState += OnClientConnectionState;
    }

    private void OnDestroy()
    {
        // Unsubscribe from events when the script is destroyed
        if (InstanceFinder.ClientManager != null)
        {
            InstanceFinder.ClientManager.OnClientConnectionState -= OnClientConnectionState;
        }
    }

    private void OnClientConnectionState(ClientConnectionStateArgs obj)
    {
        // Check if the client has successfully connected
        if (obj.ConnectionState == LocalConnectionState.Started)
        {
            // Loop through each UI GameObject and activate it
            foreach (var clientUI in uiElementsForClient)
            {
                clientUI.SetActive(true);
            }
        }
    }
}
