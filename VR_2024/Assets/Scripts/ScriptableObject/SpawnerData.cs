using UnityEngine;

[CreateAssetMenu (fileName = "SpawnerData", menuName = "Data/ManagerData/SpawnerData")]
public class SpawnerData : ScriptableObject
{
    // ReSharper disable once NotAccessedField.Global
    [SerializeField] [ReadOnly] public int activeInstances;
    public IntData activeCount;
    public PrefabDataList prefabDataList;

    private void Awake()
    {
        if (activeCount == null) activeCount = CreateInstance<IntData>();
        
        ResetSpawner();
    }

    public void ResetSpawner()
    {
        activeCount.SetValue(0);
        activeInstances = activeCount.value;
    }

    public int GetAliveCount()
    {
        var count = activeCount.value;
        if (count < 0) activeCount.SetValue(0);
        return activeCount.value;
    }
    
    public void IncrementCount()
    {
        activeCount.UpdateValue(1);
        activeInstances = activeCount.value;
    }

    public void DecrementCount()
    {
        activeCount.UpdateValue(-1);
        activeInstances = activeCount.value;
    }
}