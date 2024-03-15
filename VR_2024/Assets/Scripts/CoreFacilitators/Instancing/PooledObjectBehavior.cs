using UnityEngine;

public class PooledObjectBehavior : MonoBehaviour
{
    public SpawnManager spawnManager { get; set; }
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

    private void OnEnable()
    {
        spawned = true;
    }

    private void OnDisable()
    {
        // Debug.Log($"OnDisable of {name} called. {spawnerID} NOTIFYING: {spawned}");
        if (!spawned) return;
        spawnManager.NotifyOfDeath(spawnerID);
        spawned = false;
    }

    private void OnDestroy()
    {
        // Stops errors from being thrown on closing the game        
        spawned = false;
    }
}