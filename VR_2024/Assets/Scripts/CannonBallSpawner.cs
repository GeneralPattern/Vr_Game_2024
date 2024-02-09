using System;
using UnityEngine;

public class CannonBallSpawner : SpawnManager
{
    [HideInInspector] public IntData activeInt;
    public GameObject ammoObj;

    private PrefabDataList _prefabDataList;
    private PrefabData _ammoPrefab;

    private void Awake()
    {
        spawnerData = ScriptableObject.CreateInstance<SpawnerData>();
        activeInt = ScriptableObject.CreateInstance<IntData>();
        activeInt.zeroOnEnable = true;
        _prefabDataList = ScriptableObject.CreateInstance<PrefabDataList>();
        _ammoPrefab = ScriptableObject.CreateInstance<PrefabData>();
        _ammoPrefab.prefab = ammoObj;
        _ammoPrefab.priority = 100;
        spawnerData.globalActiveInstancesCount = activeInt;
        _prefabDataList.AddToList(_ammoPrefab);
        spawnerData.prefabDataList = _prefabDataList;
    }
}
