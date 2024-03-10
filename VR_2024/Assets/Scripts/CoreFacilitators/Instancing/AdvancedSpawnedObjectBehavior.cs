using UnityEngine;

public class AdvancedSpawnedObjectBehavior : MonoBehaviour
{ 
    private WaitForFixedUpdate _wffu = new();
    
    public AdvancedSpawnManager spawnManager { get; set; }
    
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
    
    private void OnDisable()
    {
        // if (_spawnedObjectData.spawnManager is AdvancedSpawnManager advancedSpawnManager)
        {
            // advancedSpawnManager.NotifyObjectRemoval(spawnLocation);
        }
    }
}