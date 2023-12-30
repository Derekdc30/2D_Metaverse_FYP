using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenMarketUI : MonoBehaviour
{
    public GameObject uiObject;

    private void OnTriggerEnter2D(Collider2D other)
    {
        uiObject.SetActive(true);
    }
}
