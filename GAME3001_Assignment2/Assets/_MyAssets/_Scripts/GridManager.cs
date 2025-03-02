using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TileStatus
{
    UNVISITED,
    OPEN,
    CLOSED,
    IMPASSABLE,
    GOAL,
    START
};

public enum NeighbourTile
{
    TOP_TILE,
    RIGHT_TILE,
    BOTTOM_TILE,
    LEFT_TILE,
    NUM_OF_NEIGHBOUR_TILES
};

public class GridManager : MonoBehaviour
{
    [SerializeField]
    private GameObject tilePrefab;
    [SerializeField]
    private GameObject tilePanelPrefab;
    [SerializeField]
    private GameObject tilePanelParent;
    [SerializeField]
    private GameObject obstaclePrefab;
    [SerializeField]
    private GameObject planePrefab;
    [SerializeField]
    private GameObject goalPrefab;
    [SerializeField]
    private Color[] colors;

    [SerializeField]
    private bool useManhattanHeuristic = true;
    [SerializeField]
    private float baseTileCost = 1.0f;

    private GameObject[,] grid;
    private int rows = 12;
    private int columns = 16;
    private List<GameObject> obstacles = new List<GameObject>();


    public static GridManager Instance { get; private set; } // Static object of the class.

    void Awake()
    {
        if (Instance == null) // If the object/instance doesn't exist yet.
        {
            Instance = this;
            Initialize();
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances.
        }
    }

    private void Initialize()
    {
        BuildGrid();
        ConnectGrid();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(!child.gameObject.activeSelf); // toggle tiles
            }
            tilePanelParent.SetActive(!tilePanelParent.gameObject.activeSelf);

            if (Input.GetKeyDown(KeyCode.O))
            {
                Vector2 gridPosition = GetGridPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                GameObject obstacleInst = Instantiate(obstaclePrefab, new Vector3(gridPosition.x, gridPosition.y, 0f), Quaternion.identity);
                obstacleInst.GetComponent<GridIndexScript>().SetGridIndex();
                Vector2 obstacleIndex = obstacleInst.GetComponent<GridIndexScript>().GetGridIndex();
                grid[(int)obstacleIndex.y, (int)obstacleIndex.x].GetComponent<TileScript>().SetStatus(TileStatus.IMPASSABLE);
                obstacles.Add(obstacleInst);
            }

            if (Input.GetMouseButtonDown(0))
            {
                Vector2 gridPosition = GetGridPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                GameObject planeInst = Instantiate(planePrefab, new Vector3(gridPosition.x, gridPosition.y, 0f), Quaternion.identity);
                Vector2 planeIndex = planeInst.GetComponent<GridIndexScript>().GetGridIndex();
                grid[(int)planeIndex.y, (int)planeIndex.x].GetComponent<TileScript>().SetStatus(TileStatus.START);
            }

            if (Input.GetMouseButtonDown(1))
            {
                Vector2 gridPosition = GetGridPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                GameObject goalInst = Instantiate(goalPrefab, new Vector3(gridPosition.x, gridPosition.y, 0f), Quaternion.identity);
                Vector2 goalIndex = goalInst.GetComponent<GridIndexScript>().GetGridIndex();
                grid[(int)goalIndex.y, (int)goalIndex.x].GetComponent<TileScript>().SetStatus(TileStatus.GOAL);
                SetTileCost(goalIndex);
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(1);
        }
    }

    private void BuildGrid()
    {
        grid = new GameObject[rows, columns];
        int count = 0;
        float rowPos = 5.5f;
        for (int row = 0; row < rows; row++, rowPos--)
        {
            float colPos = -7.5f;
            for (int col = 0; col < columns; col++, colPos++)
            {
                GameObject tileInst = GameObject.Instantiate(tilePrefab, new Vector3(colPos, rowPos, 0f), Quaternion.identity);
                TileScript tileScript = tileInst.GetComponent<TileScript>();
                tileScript.SetColor(colors[System.Convert.ToInt32((count++ % 2 == 0))]);
                // tileInst.GetComponent<TileScript>().SetColor(colors[System.Convert.ToInt32((count++ % 2 == 0))]);
                tileInst.transform.parent = transform;
                grid[row, col] = tileInst;
                //Instantiate a new tilepanel and link to the tile instance.
                GameObject panelInst = Instantiate(tilePanelPrefab, tilePanelPrefab.transform.position, Quaternion.identity);
                panelInst.transform.parent = tilePanelParent.transform;
                RectTransform panelTransform = panelInst.GetComponent<RectTransform>();
                panelTransform.localScale = Vector3.one;
                panelTransform.anchoredPosition = new Vector3(64f * col, -64 * row);
                tileScript.tilePanel = panelInst.GetComponent<TilePanelScript>();
                tileScript.SetStatus(TileStatus.UNVISITED);
            }
            count--;
        }

        //SetTileCost(goalIndex);
    }

    private void ConnectGrid()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                TileScript tileScript = grid[row, col].GetComponent<TileScript>();
                if (row > 0) // set top neighbour if tile is not in the top row
                {
                    tileScript.SetNeighbourTile((int)NeighbourTile.TOP_TILE, grid[row - 1, col]);
                }
                if (col > columns - 1)// set right neighbour if tile is not in rightmost tile.
                {
                    tileScript.SetNeighbourTile((int)NeighbourTile.RIGHT_TILE, grid[row, col + 1]);
                }
                if (row < rows - 1) // set bottom neighbour if tile is not in bottom row
                {
                    tileScript.SetNeighbourTile((int)NeighbourTile.BOTTOM_TILE, grid[row + 1, col]);
                }
                if (col > 0) // set left neighbour if tile is not leftmost tile
                {
                    tileScript.SetNeighbourTile((int)NeighbourTile.LEFT_TILE, grid[row, col - 1]);
                }
            }
        }
    }

    public GameObject[,] GetGrid()
    {
        return grid;
    }

    // The following utility function creates the snapping to the center of a tile.
    public Vector2 GetGridPosition(Vector2 worldPosition)
    {
        float xPos = Mathf.Floor(worldPosition.x) + 0.5f;
        float yPos = Mathf.Floor(worldPosition.y) + 0.5f;
        return new Vector2(xPos, yPos);
    }

    public void SetTileCost(Vector2 targetIndices)
    {
        float distance;
        float dx = 0f;
        float dy = 0f;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                TileScript tileScript = grid[row, col].GetComponent<TileScript>();
                if (useManhattanHeuristic)
                {
                    dx = Mathf.Abs(col - targetIndices.x);
                    dy = Mathf.Abs(row - targetIndices.y);
                    distance = dx + dy;
                }
                else // euclidean
                {
                    dx = targetIndices.x - col;
                    dy = targetIndices.y - row;
                    distance = Mathf.Sqrt(dx * dx + dy * dy);
                }

                float adjustedCost = distance * baseTileCost;
                tileScript.cost = adjustedCost;
                tileScript.tilePanel.costText.text = adjustedCost.ToString("F1");
            }
        }
    }
}

