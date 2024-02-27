using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Managing.Logging;
using FishNet.Managing.Scened;

public class OpenArcadeUI : MonoBehaviour
{
    public NetworkObject nob;
    public GameObject UI;
    private void OnTriggerEnter2D(Collider2D other)
    {
        nob = other.GetComponent<NetworkObject>();
        UI.SetActive(true);
    }
    public NetworkObject getnob(){
        return nob;
    }
}
