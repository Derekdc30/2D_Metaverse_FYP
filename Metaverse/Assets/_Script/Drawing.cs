using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
        gallary.AddToGallary(savedSprite);
        UI.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SaveDrawing();
        }
    }
}
