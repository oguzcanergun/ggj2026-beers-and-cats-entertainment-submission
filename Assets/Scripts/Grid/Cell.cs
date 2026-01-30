using UnityEngine;

[System.Serializable]
public class Cell
{
    [SerializeField] private Vector2Int _coordinates;
    [SerializeField] private bool _isWalkable;
    [SerializeField] private bool _isOccupied;

    public Vector2Int Coordinates => _coordinates;
    public bool IsWalkable
    {
        get => _isWalkable;
        set => _isWalkable = value;
    }
    public bool IsOccupied
    {
        get => _isOccupied;
        set => _isOccupied = value;
    }

    public Cell(int x, int y, bool isWalkable = true)
    {
        _coordinates = new Vector2Int(x, y);
        _isWalkable = isWalkable;
        _isOccupied = false;
    }

    public Cell(Vector2Int coordinates, bool isWalkable = true)
    {
        _coordinates = coordinates;
        _isWalkable = isWalkable;
        _isOccupied = false;
    }

    public bool CanEnter()
    {
        return _isWalkable && !_isOccupied;
    }
}
