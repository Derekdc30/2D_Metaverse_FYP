using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.SceneManagement;
using FishNet.Object;
using FishNet.Object;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;

public class AuctionHandler : NetworkBehaviour
{
    [SerializeField] string imageUrl = "http://127.0.0.1:3000/user/getimage";
    public List<Texture2D> Auctionlist = new List<Texture2D>();
    public GameObject[] AuctionSlot;
    public override void OnStartClient(){
        base.OnStartClient();
        if(!base.IsOwner){
            enabled = false;
            return;
        }
        StartCoroutine(GetImageCoroutine("1_User02"));
    } 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator GetImageCoroutine(string imageId)
    {
        // Create a new form to send the image ID
        WWWForm form = new WWWForm();
        form.AddField("id", imageId);

        // Send a POST request to the server
        using (UnityWebRequest www = UnityWebRequest.Post(imageUrl, form))
        {
            // Set the appropriate headers (if needed)
            www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

            // Wait for the response
            yield return www.SendWebRequest();

            // Get the bytes of the image
            byte[] bytes = www.downloadHandler.data;

            // Create a texture and load the image data
            Texture2D texture = new Texture2D(2, 2); // The size will be replaced by the loaded image
            texture.LoadImage(bytes);

            Auctionlist.Add(texture);
            Image img = AuctionSlot[0].GetComponent<Image>();
            img.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
            
    }
}
