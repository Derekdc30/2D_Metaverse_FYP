using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class MinimapCamera : NetworkBehaviour
{
    public Vector3 offset = new Vector3(0, 0, -50);
    private Camera minimapCamera;
    private static MinimapCamera instance; // 静态变量用于存储当前应该跟随的玩家

    [SyncVar]
    private Vector3 positionForServer;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (instance == null)
        {
            instance = this; // 设置当前玩家为应该跟随的玩家
        }
        else if (instance != this)
        {
            this.enabled = false; // 禁用脚本，不再跟随其他玩家
            return;
        }

        minimapCamera = GameObject.FindGameObjectWithTag("MiniMap").GetComponent<Camera>();
        if (minimapCamera != null)
        {
            minimapCamera.transform.position = transform.position + offset;
        }
    }

    void LateUpdate()
    {
        if (instance == this)
        {
            UpdateMinimapCameraPosition(positionForServer);
        }
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