using UnityEngine;

public class Prop : MonoBehaviour
{
    [SerializeField] private Vector2Int _size = Vector2Int.one;

    public PropType Type { get; private set; }
    public Vector2Int Coordinates { get; private set; }
    public Vector2Int Size => _size;

    public void Initialize(PropData data, Grid grid)
    {
        Type = data.Type;
        Coordinates = data.Coordinates;

        // Mark all occupied cells as unwalkable
        for (int x = 0; x < _size.x; x++)
        {
            for (int y = 0; y < _size.y; y++)
            {
                var cell = grid.GetCell(Coordinates.x + x, Coordinates.y + y);
                if (cell != null)
                {
                    cell.IsWalkable = false;
                }
            }
        }

        // Position at center of occupied cells
        var bottomLeft = grid.GetCellCenterWorld(Coordinates);
        var topRight = grid.GetCellCenterWorld(Coordinates.x + _size.x - 1, Coordinates.y + _size.y - 1);
        transform.position = (bottomLeft + topRight) / 2f;
    }
}
