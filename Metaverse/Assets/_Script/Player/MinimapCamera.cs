using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class MinimapCamera : NetworkBehaviour
{
    public Vector3 offset = new Vector3(0, 0, -50); // Adjust as needed
    private Camera minimapCamera;  // Camera for the minimap

    [SyncVar]
    private Vector3 positionForServer;  // Use Vector3 to synchronize position instead of Transform

    public override void OnStartClient()
    {
        base.OnStartClient();

        // Ensure the script is only active for the local player
        if (!IsOwner)
        {
            // Disable the script if this isn't the local player
            this.enabled = false;
            return;
        }

        minimapCamera = GameObject.FindGameObjectWithTag("MiniMap").GetComponent<Camera>();
        // Adjust the minimap camera's initial position
        if (minimapCamera != null)
        {
            minimapCamera.transform.position = transform.position + offset;
        }
    }

    void LateUpdate()
    {
        UpdateMinimapCameraPosition(positionForServer);
    }

    [ObserversRpc]
    private void UpdateMinimapCameraPosition(Vector3 newPosition)
    {
        if (minimapCamera != null)
        {
            minimapCamera.transform.position = newPosition;
        }
    }
}
