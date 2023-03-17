using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class MapPiece : IEquatable<MapPiece>
{
    public int col { get; protected set; }
    public int row { get; protected set; }

    public MapPiece(int newCol = 0, int newRow = 0)
    {
        col = newCol;
        row = newRow;
    }

    public bool Equals(MapPiece other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return col == other.col && row == other.row;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((MapPiece)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(col, row);
    }
}

public class GameManager : MonoBehaviour
{
    public int rows = 30;
    public int cols = 40;
    public float speed = 3f;
    public int numOfSnakes = 4;
    public int startLengthOfSnake = 4;
    public List<Color> snakeColors;

    private List<KeySet> keySets;
    private List<MapGrid> mapGrids;
    private List<MapPiece> availableGrids;
    private int totalSnakes;
    private MoveDirection startDirection;

    [SerializeField] private Camera camera;
    [SerializeField] private Transform gridsSpawnParent;
    [SerializeField] private GameObject gridPrefab;
    [SerializeField] private Snake snakePrefab;
    [SerializeField] private TextMeshProUGUI gameStateText;
    [SerializeField] private GameObject gameOverText;

    void Start()
    {
        keySets = new List<KeySet>
        {
            new KeySet(KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D),
            new KeySet(KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow),
            new KeySet(KeyCode.T, KeyCode.G, KeyCode.F, KeyCode.H),
            new KeySet(KeyCode.I, KeyCode.K, KeyCode.J, KeyCode.L)
        };

        startDirection = MoveDirection.Right;
        camera.orthographicSize = rows > cols ? rows / 2 + 1 : cols / 2 + 1;
        InitializeMap();
        InitializeSnakes();
        SpawnApple();
    }

    private void InitializeMap()
    {
        //Add rows and cols for borders
        rows += 2;
        cols += 2;

        mapGrids = new List<MapGrid>();
        availableGrids = new List<MapPiece>();

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                GameObject go = Instantiate(gridPrefab,
                    new Vector3(c - cols / 2, r - rows / 2),
                    Quaternion.identity,
                    gridsSpawnParent);

                MapGrid mg = go.GetComponent<MapGrid>();
                if (r == 0 || c == 0 || r == rows - 1 || c == cols - 1)
                {
                    mg.UpdateGridState(MapGridState.Unavailable);
                }
                else
                {
                    mg.UpdateGridState(MapGridState.Available);
                    availableGrids.Add(new MapPiece(c, r));
                }

                mapGrids.Add(mg);
            }
        }
    }

    //void InitialieMap2()
    //{
    // EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    // EntityArchetype entityArchetype = entityManager.CreateArchetype(
    //     typeof(Transform),
    //     typeof(MeshRenderer),
    //     typeof(MapGridState));
    // NativeArray<Entity> mapGrids = new NativeArray<Entity>((rows + 2) * (cols + 2), Allocator.Temp);
    // entityManager.CreateEntity(entityArchetype, mapGrids);
    //}

    private void InitializeSnakes()
    {
        //Spawn the snakes on the same column but with one row in between
        for (int i = 0; i < numOfSnakes; i++)
        {
            Snake snake = Instantiate(snakePrefab);
            snake.Initialize(this, cols / 2, rows / 2 + numOfSnakes - 1 - 2 * i, startLengthOfSnake, startDirection,
                snakeColors[i], keySets[i], i + 1);
            totalSnakes++;
        }
    }

    public void UpdateGridState(int col, int row, MapGridState state, Color snakeColor = default)
    {
        int index = GetIndex(col, row);
        mapGrids[index].UpdateGridState(state, snakeColor);

        switch (state)
        {
            case MapGridState.Available:
                availableGrids.Add(new MapPiece(col, row));
                break;
            case MapGridState.Apple:
            case MapGridState.Unavailable:
            case MapGridState.SnakePiece:
                availableGrids.Remove(new MapPiece(col, row));
                break;
        }
    }

    public MapGridState GetGridCurrentState(int index)
    {
        return mapGrids[index].currentState;
    }

    public void SpawnApple()
    {
        if (availableGrids.Count == 0)
        {
            gameOverText.SetActive(true);
            return;
        }

        int gridIndex = Random.Range(0, availableGrids.Count);
        int index = GetIndex(availableGrids[gridIndex].col, availableGrids[gridIndex].row);
        availableGrids.RemoveAt(gridIndex);
        mapGrids[index].UpdateGridState(MapGridState.Apple);
    }

    public void DisplayLostText(int playerNum)
    {
        totalSnakes--;
        gameStateText.text += "\nPlayer" + playerNum + " lost!";

        if (totalSnakes == 0 && gameOverText != null)
        {
            gameOverText.SetActive(true);
        }
    }

    public void ReplayGame()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private int GetIndex(int col, int row)
    {
        return row * cols + col;
    }
}