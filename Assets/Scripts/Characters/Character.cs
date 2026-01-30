using UnityEngine;
using DG.Tweening;

public abstract class Character : MonoBehaviour
{
    [SerializeField] private float _moveDuration = 0.1f;

    public Vector2Int CurrentCoordinates { get; private set; }
    public bool IsMoving { get; private set; }

    protected Grid Grid { get; private set; }

    private Tween _moveTween;

    public virtual void Initialize(Vector2Int startingCoordinates)
    {
        Grid = GridManager.Instance.Grid;
        SetPositionImmediate(startingCoordinates);
    }

    protected bool TryMove(Vector2Int direction)
    {
        if (IsMoving)
            return false;

        var targetCoords = CurrentCoordinates + direction;
        var targetCell = Grid.GetCell(targetCoords);

        if (targetCell != null && targetCell.CanEnter())
        {
            MoveToPosition(targetCoords);
            return true;
        }

        return false;
    }

    private void MoveToPosition(Vector2Int targetCoords)
    {
        IsMoving = true;
        CurrentCoordinates = targetCoords;

        var endPos = Grid.GetCellCenterWorld(targetCoords);

        _moveTween?.Kill();
        _moveTween = transform
            .DOMove(endPos, _moveDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => IsMoving = false);
    }

    protected void SetPositionImmediate(Vector2Int coordinates)
    {
        _moveTween?.Kill();
        IsMoving = false;
        CurrentCoordinates = coordinates;
        transform.position = Grid.GetCellCenterWorld(coordinates);
    }

    private void OnDestroy()
    {
        _moveTween?.Kill();
    }
}
