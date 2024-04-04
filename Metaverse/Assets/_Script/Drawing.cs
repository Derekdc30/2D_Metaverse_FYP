using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class Drawing : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public Image drawingImage;
    public float brushSize = 10f;
    public Color brushColor = Color.black;

    private RectTransform rectTransform;
    private Texture2D drawingTexture;
    private Color[] drawingPixels;
    private Texture2D savedTexture;
    public GameObject UI;
    [SerializeField] string serverUrl = "http://127.0.0.1:3000/user/gallary";

    private void Start()
    {
        rectTransform = drawingImage.rectTransform;

        // Initialize the drawing texture with the size of the image
        drawingTexture = new Texture2D((int)rectTransform.rect.width, (int)rectTransform.rect.height);
        drawingImage.sprite = Sprite.Create(drawingTexture, new Rect(0, 0, drawingTexture.width, drawingTexture.height), Vector2.one * 0.5f);

        // Initialize the drawing pixels array
        drawingPixels = new Color[drawingTexture.width * drawingTexture.height];
        ClearDrawing();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Draw(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Draw(eventData.position);
    }

    private void Draw(Vector2 position)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, position, null, out localPoint))
        {
            int x = Mathf.RoundToInt(localPoint.x + rectTransform.rect.width * 0.5f);
            int y = Mathf.RoundToInt(localPoint.y + rectTransform.rect.height * 0.5f);

            if (x >= 0 && x < drawingTexture.width && y >= 0 && y < drawingTexture.height)
            {
                drawingPixels[y * drawingTexture.width + x] = brushColor;
                drawingTexture.SetPixels(drawingPixels);
                drawingTexture.Apply();
            }
        }
    }

    public void ClearDrawing()
    {
        for (int i = 0; i < drawingPixels.Length; i++)
        {
            drawingPixels[i] = Color.white;
        }
        drawingTexture.SetPixels(drawingPixels);
        drawingTexture.Apply();
    }

    public void SaveDrawing()
    {
        // Create a copy of the drawing texture to save as PNG
        savedTexture = new Texture2D(drawingTexture.width, drawingTexture.height);
        savedTexture.SetPixels(drawingTexture.GetPixels());
        savedTexture.Apply();

        // Convert the saved texture to a sprite and display it
        Sprite savedSprite = Sprite.Create(savedTexture, new Rect(0, 0, savedTexture.width, savedTexture.height), Vector2.one * 0.5f);
        GameObject Manager = GameObject.FindGameObjectWithTag("GallayManager");
        DrawingManager gallary = Manager.GetComponent<DrawingManager>();
        int temp = gallary.getindex();
        string id = temp.ToString() + "_"+PlayerPrefs.GetString("name");
        gallary.AddToGallary(savedSprite);
        StartCoroutine(UploadSprite(PlayerPrefs.GetString("name"),"0",savedSprite,id,false));
    }
    IEnumerator UploadSprite(string UserName, string mode,Sprite sprite,string id, bool auction)
    {
        // Convert the sprite to a texture
        Texture2D texture = sprite.texture;

        // Encode the texture to PNG format
        byte[] imageData = texture.EncodeToPNG();

        // Create a new WWWForm
        WWWForm form = new WWWForm();
        form.AddField("UserName",UserName);
        form.AddField("mode",mode);
        form.AddField("id",id);
        form.AddField("auction",auction.ToString());
        if(mode =="0"){
            form.AddBinaryData("image", imageData, "image.png", "image/png");
        }
        

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SaveDrawing();
        }
    }
}
