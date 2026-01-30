using UnityEngine;
using Game.Grid;

namespace Game.Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private LevelConfig _currentLevelConfig;
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private float _cameraPadding = 1f;

        private void Start()
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            if (_currentLevelConfig == null)
            {
                Debug.LogWarning("[GameManager] No LevelConfig assigned. Grid will not be initialized.");
                return;
            }

            if (GridManager.Instance == null)
            {
                Debug.LogError("[GameManager] GridManager instance not found. Make sure GridManager is in the scene.");
                return;
            }

            GridManager.Instance.Init(_currentLevelConfig);
            SetupCamera();
        }

        private void SetupCamera()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }

            if (_mainCamera == null)
            {
                Debug.LogWarning("[GameManager] No camera found. Camera setup skipped.");
                return;
            }

            var grid = GridManager.Instance.Grid;
            float gridWorldWidth = grid.GetTotalWidth();
            float gridWorldHeight = grid.GetTotalHeight();

            // Center of the grid (XY plane)
            float centerX = grid.Origin.x + gridWorldWidth * 0.5f;
            float centerY = grid.Origin.y + gridWorldHeight * 0.5f;

            // Position camera X and Y to center on grid, keep Z
            var camPos = _mainCamera.transform.position;
            _mainCamera.transform.position = new Vector3(centerX, centerY, camPos.z);

            // Set orthographic size to fit the grid
            if (_mainCamera.orthographic)
            {
                float screenAspect = (float)Screen.width / Screen.height;
                float gridAspect = gridWorldWidth / gridWorldHeight;

                float orthoSize;
                if (gridAspect > screenAspect)
                {
                    // Grid is wider than screen - fit by width
                    orthoSize = (gridWorldWidth / screenAspect) * 0.5f;
                }
                else
                {
                    // Grid is taller than screen - fit by height
                    orthoSize = gridWorldHeight * 0.5f;
                }

                _mainCamera.orthographicSize = orthoSize + _cameraPadding;
            }

            Debug.Log($"[GameManager] Camera positioned at center of {gridWorldWidth}x{gridWorldHeight} grid");
        }
    }
}
