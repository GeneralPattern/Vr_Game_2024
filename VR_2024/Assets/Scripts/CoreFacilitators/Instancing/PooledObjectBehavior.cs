using UnityEngine;
using UnityEngine.Events;

public class PooledObjectBehavior : MonoBehaviour
{
    public UnityEvent onSpawn;
    public SpawnManager spawnManager { get; set; }
    public string spawnerID { get; set; }
    public bool spawned { get; set; }
    public bool finalSpawn { get; set; }
    public bool allowDebug { get; set; }
    
    private bool _justInstantiated;

    private void Awake()
    {
        _justInstantiated = false;
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
        if (_justInstantiated)
        {
            _justInstantiated = false;
            return;
        }
        spawned = true;
        onSpawn.Invoke();
    }

    public void InvalidateDeath()
    {
        finalSpawn = false;
        spawnManager.numToSpawn++;
        spawnManager.waitingCount++;
    }

    private void OnDisable()
    {
        if (allowDebug) Debug.Log($"OnDisable of {name} called");
        if (!spawned) return;;
        spawnManager.NotifyOfDeath(spawnerID);
        spawned = false;
    }

    private void OnDestroy()
    {
        // Stops errors from being thrown on closing the game        
        spawned = false;
    }
}