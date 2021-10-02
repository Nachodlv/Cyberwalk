using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.LevelGenerator;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
class LevelPool
{
    public ModularTerrain[] modularTerrain;
    public float minimumMeters;
}

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private LevelPool[] levels;
    [SerializeField] private Transform startingSpawnPoint;
    [SerializeField] private float percentageWalkToSpawn = 0.5f;
    [SerializeField] private int maximumLevelsSpawned = 2;

    private ModularTerrain _currentModularTerrain;
    private List<GameObject> _modularTerrainsToDelete;
    private Vector2 _lastSpawnPoint;
    private LevelPool[] _levelsSorted;
    private Transform _levelsParent;

    private void Awake()
    {
        _modularTerrainsToDelete = new List<GameObject>();
        _levelsSorted = levels.OrderBy((levelPool => levelPool.minimumMeters)).ToArray();
        _lastSpawnPoint = startingSpawnPoint.position;
        _levelsParent = new GameObject("Levels").transform;
        SpawnLevel();
    }

    private void Update()
    {
        Vector2 playerPosition = GameMode.Singleton.PlayerCached.transform.position;
        float metersWalked = playerPosition.x - _lastSpawnPoint.x;
        float percentageWalked = metersWalked / _currentModularTerrain.length;
        Debug.Log($"Percentage walked: {percentageWalked}");
        if (percentageWalked > percentageWalkToSpawn)
        {
            SpawnLevel();
        }
    }

    private void SpawnLevel()
    {
        if (_modularTerrainsToDelete.Count >= maximumLevelsSpawned)
        {
            Destroy(_modularTerrainsToDelete[0].gameObject);
            _modularTerrainsToDelete.RemoveAt(0);
        }

        LevelPool levelPool = GetCurrentLevelPool();
        int randomTerrain = Random.Range(0, levelPool.modularTerrain.Length);
        if (_currentModularTerrain)
        {
            _lastSpawnPoint.x += _currentModularTerrain.length;
        }

        _currentModularTerrain = levelPool.modularTerrain[randomTerrain];
        _lastSpawnPoint += _currentModularTerrain.offset;
        _modularTerrainsToDelete.Add(Instantiate(_currentModularTerrain.terrain, _lastSpawnPoint, Quaternion.identity, _levelsParent));
    }

    private LevelPool GetCurrentLevelPool()
    {
        float metersWalked = GameMode.Singleton.MetersWalked;
        for (int i = _levelsSorted.Length - 1; i >= 0; i--)
        {
            if (_levelsSorted[i].minimumMeters <= metersWalked)
            {
                return _levelsSorted[i];
            }
        }

        return null;
    }

    private void OnDestroy()
    {
        if (_levelsParent)
        {
            Destroy(_levelsParent.gameObject);
        }
    }
}
