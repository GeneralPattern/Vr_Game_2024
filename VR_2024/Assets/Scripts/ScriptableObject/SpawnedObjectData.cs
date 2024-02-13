using UnityEngine;

public class SpawnedObjectData : ScriptableObject
{
    [HideInInspector] public SpawnManager spawnManager { get; set; }
    
    private Vector3Data _spawnPosition;
    public void SetSpawnPosition(Vector3 spawnPosition) { _spawnPosition.value = spawnPosition; }
    public Vector3 GetSpawnPosition() { return _spawnPosition.value; }
    
    private QuaternionData _spawnRotation;
    public void SetSpawnRotation(Quaternion spawnRotation) { _spawnRotation.value = spawnRotation; }
    public Quaternion GetSpawnRotation() { return _spawnRotation.value; }
    
    private void OnEnable()
    {
        if (_spawnPosition == null) _spawnPosition = CreateInstance<Vector3Data>();
        if (_spawnRotation == null) _spawnRotation = CreateInstance<QuaternionData>();
    }
}