using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.SceneManagement;
using FishNet.Object;
using TMPro;
using UnityEngine.UI;
public class DrawingManager : NetworkBehaviour
{
    public List<Sprite> Gallary = new List<Sprite>();
    public GameObject whiteboard;
    public GameObject[] GallarySlot;
    public override void OnStartClient(){
        base.OnStartClient();
        if(!base.IsOwner){
            enabled = false;
            return;
        }

    }    
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddToGallary(Sprite image){
        if(Gallary.Count<6){
            Gallary.Add(image);
        }
        else{
            return;
        }
        GameObject Holder = GameObject.FindGameObjectWithTag("GallaryHolder");
        Image slot = GallarySlot[Gallary.Count-1].GetComponent<Image>();
        slot.sprite = image;
    }
    public void Buyslot(){
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        Economic economic = playerObject.GetComponent<Economic>();
        if(int.Parse(GameObject.FindWithTag("MoneyText").GetComponent<TextMeshProUGUI>().text.Substring(1))>= 20){
            economic.SyncMoneyroutine("20","2",PlayerPrefs.GetString("name"));
            whiteboard.SetActive(true);
        }
    }
}
