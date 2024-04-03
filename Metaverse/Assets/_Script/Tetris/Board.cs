using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Object;
using FishNet.Managing.Logging;
using FishNet.Managing.Scened;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    [SerializeField] private CanvasGroup gameOver;
    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);
    public TextMeshProUGUI ScoreText;
    private int Score = 0;
    private int _stackedSceneHandle;
    public bool SceneStack = false;
    private GameObject playerObject;
    private bool gameoverflag= false;

    public RectInt Bounds {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        Score = 0;
        UpdateScore(Score);
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < tetrominoes.Length; i++) {
            tetrominoes[i].Initialize();
        }
    }
    private void OnTriggerEnter2D(Collider2D other){
        Debug.Log(gameoverflag +" test end" );
        NetworkObject nob = other.GetComponent<NetworkObject>();
        if (nob != null)
        {
            LoadScene(nob, true);
        }
    }
    void LoadScene(NetworkObject nob, bool entering)
    {
        if (!nob.Owner.IsActive) { return; }
        if(gameoverflag){
            SceneLookupData lookup;
            lookup = new SceneLookupData(_stackedSceneHandle,"Arcade");
            SceneLoadData sld =  new SceneLoadData(lookup);
            sld.Options.AllowStacking = true;
            sld.MovedNetworkObjects = new NetworkObject[] {nob};
            sld.ReplaceScenes = ReplaceOption.All;
            InstanceFinder.SceneManager.LoadConnectionScenes(nob.Owner,sld);
        }
    }
    private void OnDestroy()
    {
        if (InstanceFinder.SceneManager != null) InstanceFinder.SceneManager.OnLoadEnd -= SceneManager_OnLoadEnd;
    }
    private void Start()
    {
        InstanceFinder.SceneManager.OnLoadEnd += SceneManager_OnLoadEnd;
        gameOver.alpha = 0f;
        gameOver.interactable = false;
        playerObject = GameObject.FindGameObjectWithTag("Player");
        playerObject.SetActive(false);
        Camera playerCamera = playerObject.GetComponentInChildren<Camera>();
        GameObject targetObject = GameObject.FindWithTag("Tetris_board");
        if (targetObject != null)
        {
            playerCamera.transform.LookAt(targetObject.transform);
        }
        SpawnPiece();
    }
    public void SpawnPiece()
    {
        int random = Random.Range(0, tetrominoes.Length);
        TetrominoData data = tetrominoes[random];

        activePiece.Initialize(this, spawnPosition, data);

        if (IsValidPosition(activePiece, spawnPosition)) {
            Set(activePiece);
        } else {
            GameOver();
        }
    }

    public void GameOver()
    {
        playerObject.SetActive(true);
        tilemap.ClearAllTiles();
        Score = 0;
        UpdateScore(Score);
        this.enabled = false;
        gameOver.interactable = true;
        StartCoroutine(Fade(gameOver, 1f, 1f));
        if(Score>=1000){
            Economic economic = playerObject.GetComponent<Economic>();
            economic.SyncMoneyroutine((Score/200).ToString(),"2",PlayerPrefs.GetString("name"));
        }
        gameoverflag = true;
        // Do anything else you want on game over here..
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        // The position is only valid if every cell is valid
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            // An out of bounds tile is invalid
            if (!bounds.Contains((Vector2Int)tilePosition)) {
                return false;
            }

            // A tile already occupies the position, thus invalid
            if (tilemap.HasTile(tilePosition)) {
                return false;
            }
        }

        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        // Clear from bottom to top
        while (row < bounds.yMax)
        {
            // Only advance to the next row if the current is not cleared
            // because the tiles above will fall down when a row is cleared
            if (IsLineFull(row)) {
                LineClear(row);
                UpdateScore(Score+=10); //1000
            } else {
                row++;
            }
        }
    }

    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            // The line is not full if a tile is missing
            if (!tilemap.HasTile(position)) {
                return false;
            }
        }

        return true;
    }

    public void LineClear(int row)
    {
        RectInt bounds = Bounds;

        // Clear all tiles in the row
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }

        // Shift every row above down one
        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }

            row++;
        }
    }
    public void UpdateScore(int score){
        ScoreText.text = score.ToString();
    }
    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        float duration = 0.5f;
        float from = canvasGroup.alpha;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = to;
    }
    private void SceneManager_OnLoadEnd(SceneLoadEndEventArgs obj)
    {
        if (!obj.QueueData.AsServer) return;
        if (!SceneStack) return;
        if (_stackedSceneHandle != 0) return;
        if (obj.LoadedScenes.Length > 0) _stackedSceneHandle = obj.LoadedScenes[0].handle;
    }
}
