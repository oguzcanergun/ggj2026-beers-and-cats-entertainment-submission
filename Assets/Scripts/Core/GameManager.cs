using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LevelConfig _currentLevelConfig;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _cameraPadding = 1f;
    [SerializeField] private Player _playerPrefab;

    public Player Player { get; private set; }

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
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (_playerPrefab == null)
        {
            Debug.LogWarning("[GameManager] No player prefab assigned. Player will not be spawned.");
            return;
        }

        Player = Instantiate(_playerPrefab);
        Player.Initialize(_currentLevelConfig.PlayerStartingCoordinates);
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

        float centerX = grid.Origin.x + gridWorldWidth * 0.5f;
        float centerY = grid.Origin.y + gridWorldHeight * 0.5f;

        var camPos = _mainCamera.transform.position;
        _mainCamera.transform.position = new Vector3(centerX, centerY, camPos.z);

        if (_mainCamera.orthographic)
        {
            float screenAspect = (float)Screen.width / Screen.height;
            float gridAspect = gridWorldWidth / gridWorldHeight;

            float orthoSize;
            if (gridAspect > screenAspect)
            {
                orthoSize = (gridWorldWidth / screenAspect) * 0.5f;
            }
            else
            {
                orthoSize = gridWorldHeight * 0.5f;
            }

            _mainCamera.orthographicSize = orthoSize + _cameraPadding;
        }

        Debug.Log($"[GameManager] Camera positioned at center of {gridWorldWidth}x{gridWorldHeight} grid");
    }
}
