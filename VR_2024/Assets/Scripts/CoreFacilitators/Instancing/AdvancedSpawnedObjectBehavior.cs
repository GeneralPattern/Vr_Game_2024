using UnityEngine;

public class AdvancedSpawnedObjectBehavior : MonoBehaviour
{/*
    public AdvancedSpawnManager spawnManager { get; set; }
    public string spawnerID { get; set; }
    public bool spawned { get; set; }

    private void Awake()
    {
        spawned = false;
    }

    public void SetSpawnDelay(float respawnTime)
    {
        if (spawnManager == null)
        {
            Debug.LogWarning("SpawnManager is null on " + name + " SpawnedObjectBehavior.");
            return;
        }
        spawnManager.SetSpawnDelay(respawnTime);
    }

    public void TriggerRespawn()
    {
        if (spawnManager == null)
        {
            Debug.LogWarning("SpawnManager is null" + name + " SpawnedObjectBehavior.");
            return;
        }
        spawnManager.StartSpawn(1);
    }
    
    // private void OnEnable()
    // {
    //     if (!spawned) return;
    //     spawnManager.NotifyOfSpawn(spawnerID);
    // }
    
    private void OnDisable()
    {
        if (!spawned) return;
        spawnManager.NotifyOfDeath(spawnerID);
        spawned = false;
    }
*/}