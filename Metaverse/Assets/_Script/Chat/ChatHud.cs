using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatHud : MonoBehaviour
{
    public Button openButton;
    private void Awake()
    {
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        openButton.gameObject.SetActive(false);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        openButton.gameObject.SetActive(true);
    }
}
