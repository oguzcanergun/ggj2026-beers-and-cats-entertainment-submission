using System.Collections.Generic;
using UnityEngine;

public class PropManager : MonoBehaviour
{
    public static PropManager Instance { get; private set; }

    [System.Serializable]
    public class PropPrefabEntry
    {
        public PropType Type;
        public Prop Prefab;
    }

    [SerializeField] private List<PropPrefabEntry> _propPrefabs;

    private Transform _propsParent;
    private List<Prop> _spawnedProps = new List<Prop>();
    private Dictionary<PropType, Prop> _prefabLookup;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        BuildPrefabLookup();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void BuildPrefabLookup()
    {
        _prefabLookup = new Dictionary<PropType, Prop>();
        foreach (var entry in _propPrefabs)
        {
            _prefabLookup[entry.Type] = entry.Prefab;
        }
    }

    public void SpawnProps(LevelConfig config)
    {
        ClearProps();

        if (config.Props == null || config.Props.Count == 0)
            return;

        _propsParent = new GameObject("Props").transform;
        _propsParent.SetParent(transform);

        var grid = GridManager.Instance.Grid;

        foreach (var propData in config.Props)
        {
            if (!_prefabLookup.TryGetValue(propData.Type, out var prefab) || prefab == null)
            {
                Debug.LogWarning($"[PropManager] No prefab found for prop type: {propData.Type}");
                continue;
            }

            var prop = Instantiate(prefab, _propsParent);
            prop.Initialize(propData, grid);
            _spawnedProps.Add(prop);
        }

        Debug.Log($"[PropManager] Spawned {_spawnedProps.Count} props");
    }

    public void ClearProps()
    {
        if (_propsParent != null)
        {
            Destroy(_propsParent.gameObject);
            _propsParent = null;
        }

        _spawnedProps.Clear();
    }
}
