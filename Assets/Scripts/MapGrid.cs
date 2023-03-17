using UnityEngine;

public enum MapGridState
{
    Available,
    Apple,
    SnakePiece,
    Unavailable //for barriers like rocks or boundaries
}

public class MapGrid : MonoBehaviour
{
    public MapGridState currentState { get; private set; }
    public SpriteRenderer renderer;

    public Color availableColor;
    public Color appleColor;
    public Color unavailableColor;

    // Start is called before the first frame update
    void Start()
    {
        if (renderer == null)
        {
            renderer = GetComponent<SpriteRenderer>();
        }
    }

    public void UpdateGridState(MapGridState state, Color snakeColor = default)
    {
        currentState = state;
        switch (currentState)
        {
            case MapGridState.Available:
                renderer.color = availableColor;
                break;
            case MapGridState.Apple:
                renderer.color = appleColor;
                break;
            case MapGridState.SnakePiece:
                renderer.color = snakeColor;
                break;
            case MapGridState.Unavailable:
                renderer.color = unavailableColor;
                break;
        }
    }
}