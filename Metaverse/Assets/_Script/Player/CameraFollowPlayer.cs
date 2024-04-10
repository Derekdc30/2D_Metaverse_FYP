using UnityEngine;
using FishNet.Object;

public class CameraFollowPlayer : NetworkBehaviour
{
    public Vector3 offset = new Vector3(0, 0, -10); // Adjust as needed

    public override void OnStartClient()
    {
        base.OnStartClient();

        // Ensure the script is only active for the local player
        if (!IsOwner)
        {
            // If this isn't the local player, disable the script (or just the camera movement part)
            this.enabled = false;
            return;
        }

        // Since this is the local player, optionally adjust the main camera settings here
        // For example, setting the initial camera position
        if (Camera.main != null)
        {
            Camera.main.transform.position = transform.position + offset;
        }
    }

    void LateUpdate()
    {
        // No need to check IsOwner here since the script is disabled for non-local players
        if (Camera.main != null)
        {
            Camera.main.transform.position = transform.position + offset;
        }
    }
}