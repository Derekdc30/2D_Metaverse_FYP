using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ChatHud : MonoBehaviour
{
    public GameObject chatPanel, textObject;
    public TMP_InputField chatBox;
    public int maxMessages = 25;

    [SerializeField]
    List<Message> messagesList = new List<Message>();


    private void Update()
    {
        if(!chatBox.isFocused)
        {
            //Test
            /*
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendMessageToChat("3333");
            }
            */
        }
        
        if(chatBox.text != "")
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                SendMessageToChat(chatBox.text);
                chatBox.text = "";
            }
        }else
        {
            if(!chatBox.isFocused && Input.GetKeyDown(KeyCode.Return))
            {
                chatBox.ActivateInputField();
            }
        }
        
    }
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

    public void SendMessageToChat(string message)
    {
        if (messagesList.Count >= maxMessages)
        {
            Destroy(messagesList[0].textObject.gameObject);
            messagesList.Remove(messagesList[0]);
        }
        Message newMessage = new Message();
        newMessage.text = message;
        GameObject newText = Instantiate(textObject, chatPanel.transform);
        newMessage.textObject = newText.GetComponent<Text>();
        newMessage.textObject.text = newMessage.text;
        messagesList.Add(newMessage);
    }

    public class Message
    {
        public string text;
        public Text textObject;
    }

}

