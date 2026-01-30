# Project Context for Claude

## Project Overview
- **Engine:** Unity 6 (6000.0.66f1)
- **Render Pipeline:** Universal Render Pipeline (URP)
- **Input:** Legacy Input System (Input.GetKeyDown)
- **Game Type:** Top-down 2D game on XY plane (camera looks down Z axis)
- **Namespaces:** None (all classes are in global namespace)

---

## Implemented Systems

### Grid System (`Assets/Scripts/Grid/`)

#### Cell.cs
- Pure data class representing a single grid cell
- Properties: `Coordinates` (Vector2Int), `IsWalkable`, `IsOccupied`
- Method: `CanEnter()` returns true if walkable and not occupied

#### Grid.cs
- Manages 2D array of cells
- Properties: `Width`, `Height`, `CellSize`, `CellSpacing`, `Origin`
- Key methods:
  - `GetCell(x, y)` / `GetCell(Vector2Int)`
  - `GetNeighbours(cell)` - returns 4-directional neighbors (up/down/left/right)
  - `GetWalkableNeighbours(cell)`
  - `GetCellCenterWorld(x, y)` - returns world position of cell center
  - `GetCoordinatesFromWorld(Vector3)` - converts world pos to grid coords
  - `GetTotalWidth()` / `GetTotalHeight()` - total grid size in world units

#### GridManager.cs (MonoBehaviour Singleton)
- `Instance` - static singleton reference
- `Grid` - the active Grid instance
- `[SerializeField] _cellPrefab` - prefab instantiated for each cell
- `Init(LevelConfig)` - creates grid and instantiates cell prefabs
- `OnGridInitialized` event

#### LevelConfig.cs (ScriptableObject)
- Create via: Right-click → Create → Game → Level Config
- Fields:
  - `GridWidth`, `GridHeight` (int)
  - `CellSize` (float, default 1)
  - `CellSpacing` (float, default 0.5)
  - `GridOrigin` (Vector3)
  - `PlayerStartingCoordinates` (Vector2Int)
  - `Props` (List<PropData>) - props to spawn on this level

---

### Core (`Assets/Scripts/Core/`)

#### GameManager.cs (MonoBehaviour)
- `[SerializeField] _currentLevelConfig` - assign LevelConfig asset
- `[SerializeField] _mainCamera` - optional, auto-finds Camera.main
- `[SerializeField] _cameraPadding` - extra space around grid (default 1)
- `[SerializeField] _playerPrefab` - player prefab to instantiate
- `Player` - reference to spawned player instance (read-only)
- On Start:
  1. Calls `GridManager.Instance.Init(_currentLevelConfig)`
  2. Calls `PropManager.Instance.SpawnProps(_currentLevelConfig)`
  3. Positions camera X/Y to center on grid (keeps Z)
  4. Sets orthographic size to fit grid
  5. Spawns player at `LevelConfig.PlayerStartingCoordinates`

---

### Characters (`Assets/Scripts/Characters/`)

#### Character.cs (Abstract Base Class)
- `CurrentCoordinates` - current grid position (read-only)
- `IsMoving` - true while tweening between cells
- `Grid` - protected reference to grid instance
- `_moveDuration` - serialized, default 0.1s (snappy)
- `Initialize(Vector2Int startingCoordinates)` - virtual, sets up grid reference and position
- `TryMove(Vector2Int direction)` - protected, validates and tweens to target cell (DOTween), returns bool
- `SetPositionImmediate(Vector2Int coordinates)` - protected, sets position instantly

#### Player.cs (Derives from Character)
- Handles WASD + Arrow key input in Update()
- Calls `TryMove()` for 4-directional movement

#### AICharacter.cs (Derives from Character)
- Base class for AI-controlled characters
- AI behavior to be implemented

---

### Props System (`Assets/Scripts/Props/`)

#### PropType.cs (Enum)
- `Table` - first prop type

#### PropData.cs (Serializable Class)
- `Type` (PropType)
- `Coordinates` (Vector2Int) - bottom-left corner
- `Size` (Vector2Int) - width x height, default (1,1)

#### Prop.cs (MonoBehaviour)
- `Type`, `Coordinates`, `Size` - read-only properties
- `Initialize(PropData, Grid)` - marks occupied cells as unwalkable, positions at center

#### PropManager.cs (MonoBehaviour Singleton)
- `Instance` - static singleton reference
- `_propPrefabs` - list of PropType → Prefab mappings
- `SpawnProps(LevelConfig)` - spawns all props from config
- `ClearProps()` - destroys spawned props

---

## Coordinate System
- **Grid is on XY plane** (not XZ)
- Camera looks down negative Z axis
- Grid origin is bottom-left corner
- Cell (0,0) is at origin + half cell size offset

## World Position Calculation
```
CellStride = CellSize + CellSpacing
CellCenter(x,y) = Origin + (x * CellStride + CellSize/2, y * CellStride + CellSize/2, 0)
```

---

## Scene Setup Requirements
1. GameObject with `GridManager` component (assign cell prefab)
2. GameObject with `PropManager` component (assign prop prefabs)
3. GameObject with `GameManager` component (assign LevelConfig + player prefab)
4. Camera set to Orthographic
5. Player prefab with `Player` component (spawned automatically by GameManager)
6. Prop prefabs with `Prop` component

---

## Folder Structure
```
Assets/
├── Scripts/
│   ├── Characters/
│   │   ├── Character.cs
│   │   ├── Player.cs
│   │   └── AICharacter.cs
│   ├── Core/
│   │   └── GameManager.cs
│   ├── Props/
│   │   ├── PropType.cs
│   │   ├── PropData.cs
│   │   ├── Prop.cs
│   │   └── PropManager.cs
│   └── Grid/
│       ├── Cell.cs
│       ├── Grid.cs
│       ├── GridManager.cs
│       └── LevelConfig.cs
├── Prefabs/
│   └── (cell prefab here)
├── Resources/
│   └── Configs/
│       └── Levels/
│           └── (LevelConfig assets)
└── Scenes/
    └── SampleScene.unity
```

---

## Notes
- Cell prefab is assigned on GridManager (same for all levels)
- Grid size/spacing configured per-level via LevelConfig
- Player and AI characters will share the same grid
- 4-directional movement only (no diagonals)
