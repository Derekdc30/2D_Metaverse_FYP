using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class Player : NetworkBehaviour
{
    private Transform LocalPostion;
    [SyncVar]
    private Transform PostionForServer;

    [ServerRpc]
    private void SendPosToServer()
    {
        PostionForServer = LocalPostion;
    }
}
