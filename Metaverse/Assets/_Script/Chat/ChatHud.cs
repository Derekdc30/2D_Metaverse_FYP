using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatHud : MonoBehaviour
{
    public GameObject chatPanel, textObject;
    public TMP_InputField chatBox;
    public ChatNetworkManager networkManager;
    public int maxMessages = 25;

    [SerializeField]
    private List<GameObject> messagesList = new List<GameObject>();


    private void Awake()
    {
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    // This method will be called by ChatNetworkManager to display messages
    public void ReceiveMessage(string message)
    {
        if (messagesList.Count >= maxMessages)
        {
            Destroy(messagesList[0]);
            messagesList.RemoveAt(0);
        }

        GameObject newText = Instantiate(textObject, chatPanel.transform);
        newText.GetComponent<TMP_Text>().text = message;
        newText.SetActive(true);

        messagesList.Add(newText);
    }
}
