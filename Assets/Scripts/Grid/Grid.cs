using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private static readonly Vector2Int[] Directions =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    private readonly Cell[,] _cells;

    public int Width { get; }
    public int Height { get; }
    public float CellSize { get; }
    public float CellSpacing { get; }
    public Vector3 Origin { get; }

    private float CellStride => CellSize + CellSpacing;

    public Grid(int width, int height, float cellSize, float cellSpacing, Vector3 origin)
    {
        Width = width;
        Height = height;
        CellSize = cellSize;
        CellSpacing = cellSpacing;
        Origin = origin;

        _cells = new Cell[width, height];
        InitializeCells();
    }

    private void InitializeCells()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                _cells[x, y] = new Cell(x, y);
            }
        }
    }

    public Cell GetCell(int x, int y)
    {
        if (!IsValidCoordinate(x, y))
        {
            return null;
        }
        return _cells[x, y];
    }

    public Cell GetCell(Vector2Int coordinates)
    {
        return GetCell(coordinates.x, coordinates.y);
    }

    public bool TryGetCell(int x, int y, out Cell cell)
    {
        cell = GetCell(x, y);
        return cell != null;
    }

    public bool IsValidCoordinate(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public bool IsValidCoordinate(Vector2Int coordinates)
    {
        return IsValidCoordinate(coordinates.x, coordinates.y);
    }

    public List<Cell> GetNeighbours(Cell cell)
    {
        return GetNeighbours(cell.Coordinates.x, cell.Coordinates.y);
    }

    public List<Cell> GetNeighbours(int x, int y)
    {
        var neighbours = new List<Cell>();

        foreach (var direction in Directions)
        {
            int neighbourX = x + direction.x;
            int neighbourY = y + direction.y;

            if (TryGetCell(neighbourX, neighbourY, out var neighbour))
            {
                neighbours.Add(neighbour);
            }
        }

        return neighbours;
    }

    public List<Cell> GetWalkableNeighbours(Cell cell)
    {
        var neighbours = GetNeighbours(cell);
        neighbours.RemoveAll(n => !n.CanEnter());
        return neighbours;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return Origin + new Vector3(x * CellStride, y * CellStride, 0f);
    }

    public Vector3 GetWorldPosition(Vector2Int coordinates)
    {
        return GetWorldPosition(coordinates.x, coordinates.y);
    }

    public Vector3 GetCellCenterWorld(int x, int y)
    {
        return Origin + new Vector3(
            x * CellStride + CellSize * 0.5f,
            y * CellStride + CellSize * 0.5f,
            0f
        );
    }

    public Vector3 GetCellCenterWorld(Vector2Int coordinates)
    {
        return GetCellCenterWorld(coordinates.x, coordinates.y);
    }

    public Vector2Int GetCoordinatesFromWorld(Vector3 worldPosition)
    {
        var localPos = worldPosition - Origin;
        return new Vector2Int(
            Mathf.FloorToInt(localPos.x / CellStride),
            Mathf.FloorToInt(localPos.y / CellStride)
        );
    }

    public float GetTotalWidth()
    {
        return Width * CellSize + (Width - 1) * CellSpacing;
    }

    public float GetTotalHeight()
    {
        return Height * CellSize + (Height - 1) * CellSpacing;
    }
}
