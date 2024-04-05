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
    [SerializeField] string serverUrl = "http://127.0.0.1:3000/user/gallary";
    public List<Texture2D> Auctionlist = new List<Texture2D>();
    public GameObject[] AuctionSlot;
    public GameObject pricefield;
    public GameObject Buyui;
    public override void OnStartClient(){
        base.OnStartClient();
        if(!base.IsOwner){
            enabled = false;
            return;
        }
        
    } 
    private void Awake() {
        StartCoroutine(GetImageCoroutine("1_User02"));
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void openbuy(){
        Buyui.SetActive(true);
    }
    public void Buy(){
        string price = pricefield.GetComponent<TMP_InputField>().text;
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        Economic economic = playerObject.GetComponent<Economic>();
        if(int.Parse(GameObject.FindWithTag("MoneyText").GetComponent<TextMeshProUGUI>().text.Substring(1))>= int.Parse(price)){
            economic.SyncMoneyroutine(price,"2",PlayerPrefs.GetString("name"));
            StartCoroutine(UploadSprite(PlayerPrefs.GetString("name"),"1","","",false));
        }
        Buyui.SetActive(false);
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
            www.SetRequestHeader("Content-Type", "image/png");

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
    IEnumerator UploadSprite(string UserName, string mode,string sprite,string id, bool auction)
    {
        // Convert the sprite to a texture
        //Texture2D texture = sprite.texture;

        // Encode the texture to PNG format
        //byte[] imageData = texture.EncodeToPNG();

        // Create a new WWWForm
        WWWForm form = new WWWForm();
        form.AddField("UserName",UserName);
        form.AddField("mode",mode);
        form.AddField("id",id);
        form.AddField("auction",auction.ToString());

        

        // Create the UnityWebRequest using the form data
        using (UnityWebRequest www = UnityWebRequest.Post(serverUrl, form))
        {
            // Send the request
            yield return www.SendWebRequest();

            // Check for errors
            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError("Error uploading sprite: " + www.error);
            }
            else
            {
                if (www.responseCode == 200) // Check if the request was successful
                {
                    Debug.Log("Sprite uploaded successfully");
                }
                else
                {
                    Debug.LogError("Error " + www.responseCode + ": " + www.downloadHandler.text);
                }
            }
        }
    }
}
