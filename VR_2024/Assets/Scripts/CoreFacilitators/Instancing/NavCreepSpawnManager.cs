using UnityEngine;

public class NavCreepSpawnManager : SpawnManager
{
    public Transform pathingTarget;

    protected override void Spawn(GameObject obj)
    {
        obj.GetComponent<NavAgentBehavior>().destination = pathingTarget;
        obj.SetActive(true);
        obj.GetComponent<NavAgentBehavior>().Setup(pathingTarget);
        base.Spawn(obj);
    }
    
    protected override GameObject FetchPrefab(PrefabData data)
    {
        var creepPrefabData = (CreepPrefabData)data;
        creepPrefabData.creepData.totalSpawned += 1;
        return base.FetchPrefab(creepPrefabData);
    }
}