/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Spawner
{
    public string id;
    public Transform spawnLocation;
    public int spawnCount;
    public int spawnLimit;
    public int GetAliveCount() { return spawnCount; }
    public void IncrementCount() { spawnCount++; }
    public void DecrementCount() { spawnCount--; }
}

public class AdvancedSpawnManager : SpawnManager
{
    [SerializeField] [ReadOnly] private int globalSpawnCount;
    public int globalSpawnLimit, instanceSpawnLimit; 
    public float minSpawnRate, maxSpawnRate;

    public List<Spawner> availableSpawners;

    protected override void Start()
    {
        base.Start();
        InvokeRepeating(nameof(Spawn), minSpawnRate, Random.Range(minSpawnRate, maxSpawnRate));
    }

    protected void Spawn()
    {
        if (globalSpawnCount >= globalSpawnLimit) return;
        if (availableSpawners.Count == 0) return;

        int spawnLocationIndex = Random.Range(0, availableSpawners.Count);

        // Call your Spawn method from the SpawnManager class here. Make sure to pass the correct prefab and the position of availableSpawnLocations[spawnLocationIndex].
        

        globalSpawnCount++; // increment the global count of spawned objects
        availableSpawners[spawnLocationIndex].IncrementCount();

        // If the instance limit was reached for a spawn location, remove it from the list of available spawn locations.
        if (availableSpawners[spawnLocationIndex].GetAliveCount() >= instanceSpawnLimit)
        {
            availableSpawners.RemoveAt(spawnLocationIndex);
        }
    }

    protected override IEnumerator SpawnRoutine()
    {
        
    }

    public void NotifyObjectRemoval( )
    {
        spawnerData.DecrementCount(); // reduce the count for the specific spawn location
        globalSpawnCount--; // reduce the global count

        // If the spawn location became available again, add it back to the list
        if (spawnerData.GetAliveCount() < instanceSpawnLimit && !availableSpawners.Contains(spawnLocation))
        {
            availableSpawners.Add(spawnLocation);
        }
    }
}
*/