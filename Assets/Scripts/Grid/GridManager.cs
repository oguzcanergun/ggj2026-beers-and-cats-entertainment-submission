using System;
using UnityEngine;

namespace Game.Grid
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance { get; private set; }

        [SerializeField] private GameObject _cellPrefab;

        public Grid Grid { get; private set; }
        public bool IsInitialized { get; private set; }

        public event Action<Grid> OnGridInitialized;

        private Transform _cellsParent;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void Init(LevelConfig config)
        {
            if (config == null)
            {
                Debug.LogError("[GridManager] LevelConfig is null. Cannot initialize grid.");
                return;
            }

            ClearGrid();

            Grid = new Grid(
                config.GridWidth,
                config.GridHeight,
                config.CellSize,
                config.CellSpacing,
                config.GridOrigin
            );

            InstantiateCells();

            IsInitialized = true;

            Debug.Log($"[GridManager] Grid initialized: {config.GridWidth}x{config.GridHeight}, CellSize: {config.CellSize}");

            OnGridInitialized?.Invoke(Grid);
        }

        private void InstantiateCells()
        {
            if (_cellPrefab == null)
            {
                Debug.LogWarning("[GridManager] No cell prefab assigned. Skipping cell instantiation.");
                return;
            }

            _cellsParent = new GameObject("Cells").transform;
            _cellsParent.SetParent(transform);

            for (int x = 0; x < Grid.Width; x++)
            {
                for (int y = 0; y < Grid.Height; y++)
                {
                    var worldPos = Grid.GetCellCenterWorld(x, y);
                    var cellObj = Instantiate(_cellPrefab, worldPos, Quaternion.identity, _cellsParent);
                    cellObj.name = $"Cell_{x}_{y}";
                }
            }
        }

        public void ClearGrid()
        {
            if (_cellsParent != null)
            {
                Destroy(_cellsParent.gameObject);
                _cellsParent = null;
            }

            Grid = null;
            IsInitialized = false;
        }
    }
}
