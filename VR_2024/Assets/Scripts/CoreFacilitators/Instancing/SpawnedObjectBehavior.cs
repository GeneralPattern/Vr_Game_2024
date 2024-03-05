using UnityEngine;

public class SpawnedObjectBehavior : MonoBehaviour
{ 
    private SpawnedObjectData _spawnedObjectData;
    public float respawnTime;
    
    private void Awake()
    {
        _spawnedObjectData = ScriptableObject.CreateInstance<SpawnedObjectData>();
        if (GetComponent<SpawnManager>()) _spawnedObjectData.spawnManager = GetComponent<SpawnManager>();
    }
    
    public void SetSpawnManager(SpawnManager spawnManager) { _spawnedObjectData.spawnManager = spawnManager; }

    public Vector3 GetSpawnPosition() { return _spawnedObjectData.GetSpawnPosition(); }
    public void SetSpawnPosition(Vector3 spawnPosition)
    {
        _spawnedObjectData.SetSpawnPosition(spawnPosition);
        transform.position = spawnPosition;
    }

    public Quaternion GetSpawnRotation() { return _spawnedObjectData.GetSpawnRotation(); }
    public void SetSpawnRotation(Quaternion spawnRotation)
    {
        _spawnedObjectData.SetSpawnRotation(spawnRotation);
        transform.rotation = spawnRotation;
    }

    public void TriggerRespawn()
    {
        _spawnedObjectData.spawnManager.StartSpawn(_spawnedObjectData.spawnManager.numToSpawn);
    }
    
    public void SetSpawnTime()
    {
        if (_spawnedObjectData.spawnManager.GetSpawnDelay() != respawnTime)
        {
            _spawnedObjectData.spawnManager.SetSpawnDelay(respawnTime);
        }
    }
    
    private void OnDisable()
    {
        // if (_spawnedObjectData.spawnManager is AdvancedSpawnManager advancedSpawnManager)
        {
            // advancedSpawnManager.NotifyObjectRemoval(spawnLocation);
        }
    }
}