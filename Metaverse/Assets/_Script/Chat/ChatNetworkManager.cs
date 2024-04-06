using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class ChatNetworkManager : NetworkBehaviour
{
    public ChatHud chatHud;

    public void SendMessageToServer(string message)
    {
        /*
        if (IsServer) // If this check passes, you're on the server already.
        {
            Debug.Log("SendMessageToServer called on server. This should only be called from clients.");
            return;
        }
        */
        // Send to the server.
        RequestSendMessageToServer(message);
        Debug.Log(1);
    }

    [ServerRpc]
    private void RequestSendMessageToServer(string message)
    {
        BroadcastMessageToClients(message);
        Debug.Log(2);
    }

    [ObserversRpc]
    private void BroadcastMessageToClients(string message)
    {
        chatHud.ReceiveMessage(message);
        Debug.Log(3);
    }
}
