using UnityEngine;

namespace Game.Grid
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Game/Level Config")]
    public class LevelConfig : ScriptableObject
    {
        [Header("Grid Settings")]
        [SerializeField] private int _gridWidth = 10;
        [SerializeField] private int _gridHeight = 10;
        [SerializeField] private float _cellSize = 1f;
        [SerializeField] private float _cellSpacing = 0.5f;
        [SerializeField] private Vector3 _gridOrigin = Vector3.zero;

        public int GridWidth => _gridWidth;
        public int GridHeight => _gridHeight;
        public float CellSize => _cellSize;
        public float CellSpacing => _cellSpacing;
        public Vector3 GridOrigin => _gridOrigin;
    }
}
