using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ChatNetworkManager : NetworkBehaviour
{
    public TMP_InputField chatInputField;
    public GameObject chatContentPanel;  // Attach the content panel of the Scroll View
    public GameObject chatMessagePrefab;  // The prefab you created for each chat message

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("Client started and is ready.");
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && chatInputField.text.Trim() != "")
        {
            SendChatMessage(chatInputField.text);
            chatInputField.text = "";
            chatInputField.ActivateInputField();
        }
    }

    private void SendChatMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return;
        
        // Call the server RPC to send the message to all clients
        SendChatMessageServerRpc(message);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendChatMessageServerRpc(string message)
    {
        UpdateChatHistoryOnAllClients($"[Player {Owner.ClientId}]: {message}\n");
    }

    [ObserversRpc]
    private void UpdateChatHistoryOnAllClients(string message)
    {
        GameObject msg = Instantiate(chatMessagePrefab, chatContentPanel.transform);
        msg.GetComponent<Text>().text = message;
        // Ensure the chat scrolls to the bottom
        StartCoroutine(ScrollToBottom());
    }

    private IEnumerator ScrollToBottom()
    {
        // Wait for end of frame so that UI elements can update layout
        yield return new WaitForEndOfFrame();
        chatContentPanel.transform.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
    }
}
