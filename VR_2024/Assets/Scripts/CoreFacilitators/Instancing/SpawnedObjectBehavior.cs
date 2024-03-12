using System;
using UnityEngine;
using UnityEngine.Events;

public class SpawnedObjectBehavior : MonoBehaviour
{ 
    private WaitForFixedUpdate _wffu = new();
        
    public SpawnManager spawnManager { get; set; }
    public UnityEvent onSpawnEvent;
    
    public Vector3 spawnPosition { get; set; }
    public Quaternion spawnRotation { get; set; }

    public void TriggerRespawn()
    {
        if (spawnManager == null)
        {
            Debug.LogWarning("SpawnManager is null" + name + " SpawnedObjectBehavior.");
            return;
        }
        spawnManager.StartSpawn(spawnManager.numToSpawn);
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

    private void OnEnable()
    {
        onSpawnEvent.Invoke();
    }

    private void OnDisable()
    {
        // if (_spawnedObjectData.spawnManager is AdvancedSpawnManager advancedSpawnManager)
        {
            // advancedSpawnManager.NotifyObjectRemoval(spawnLocation);
        }
    }
}