using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveDirection
{
    Up,
    Down,
    Left,
    Right
}

public struct KeySet
{
    public KeyCode up;
    public KeyCode down;
    public KeyCode left;
    public KeyCode right;

    public KeySet(KeyCode up = KeyCode.W, KeyCode down = KeyCode.S, KeyCode left = KeyCode.A, KeyCode right = KeyCode.D)
    {
        this.up = up;
        this.down = down;
        this.left = left;
        this.right = right;
    }
}

public class Snake : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    public LinkedList<MapPiece> snakePieces;
    private MoveDirection currentDirection;
    private Color snakeColor;
    private KeySet keySet;
    public bool isDirectionLocked;
    private int playerNum;

    void Start()
    {
        StartCoroutine(UpdateSnake());
    }

    // Update is called once per frame
    void Update()
    {
        ChangeDirection();
    }

    public void Initialize(GameManager gm, int headCol, int headRow, int length, MoveDirection direction, Color color,
        KeySet ks, int playerNo)
    {
        gameManager = gm;
        currentDirection = direction;
        snakeColor = color;
        keySet = ks;
        playerNum = playerNo;
        isDirectionLocked = false;
        snakePieces = new LinkedList<MapPiece>();

        for (int i = 0; i < length; i++)
        {
            snakePieces.AddLast(new MapPiece(headCol - i, headRow));
            gameManager.UpdateGridState(headCol - i, headRow, MapGridState.SnakePiece, snakeColor);
        }
    }

    private IEnumerator UpdateSnake()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f / gameManager.speed);

            MoveSnake();
        }
    }

    private void MoveSnake()
    {
        MapPiece head = snakePieces.First.Value;
        MapPiece tail = snakePieces.Last.Value;
        MapPiece newHead;

        switch (currentDirection)
        {
            case MoveDirection.Up:
                newHead = new MapPiece(head.col, head.row + 1);
                break;
            case MoveDirection.Down:
                newHead = new MapPiece(head.col, head.row - 1);
                break;
            case MoveDirection.Right:
                newHead = new MapPiece(head.col + 1, head.row);
                break;
            case MoveDirection.Left:
                newHead = new MapPiece(head.col - 1, head.row);
                break;
            default:
                newHead = new MapPiece();
                break;
        }

        int newHeadIndex = newHead.row * gameManager.cols + newHead.col;
        MapGridState newHeadState = gameManager.GetGridCurrentState(newHeadIndex);

        switch (newHeadState)
        {
            case MapGridState.Unavailable:
            case MapGridState.SnakePiece:
                CleanDeadSnake();
                return;
            case MapGridState.Apple:
                gameManager.SpawnApple();
                break;
            case MapGridState.Available:
                snakePieces.RemoveLast();
                gameManager.UpdateGridState(tail.col, tail.row, MapGridState.Available);
                break;
        }

        snakePieces.AddFirst(newHead);
        gameManager.UpdateGridState(newHead.col, newHead.row, MapGridState.SnakePiece, snakeColor);

        isDirectionLocked = false;
    }

    //Change to new direction unless it's opposite to current direction 
    private void ChangeDirection()
    {
        if (!isDirectionLocked)
        {
            if (Input.GetKeyDown(keySet.up) && currentDirection != MoveDirection.Up &&
                currentDirection != MoveDirection.Down)
            {
                currentDirection = MoveDirection.Up;
                isDirectionLocked = true;
            }

            if (Input.GetKeyDown(keySet.down) && currentDirection != MoveDirection.Down &&
                currentDirection != MoveDirection.Up)
            {
                currentDirection = MoveDirection.Down;
                isDirectionLocked = true;
            }

            if (Input.GetKeyDown(keySet.left) && currentDirection != MoveDirection.Left &&
                currentDirection != MoveDirection.Right)
            {
                currentDirection = MoveDirection.Left;
                isDirectionLocked = true;
            }

            if (Input.GetKeyDown(keySet.right) && currentDirection != MoveDirection.Right &&
                currentDirection != MoveDirection.Left)
            {
                currentDirection = MoveDirection.Right;
                isDirectionLocked = true;
            }
        }
    }

    private void CleanDeadSnake()
    {
        foreach (MapPiece sp in snakePieces)
        {
            gameManager.UpdateGridState(sp.col, sp.row, MapGridState.Available);
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        gameManager.DisplayLostText(playerNum);
    }
}