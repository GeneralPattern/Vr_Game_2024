using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabDataList", menuName = "Data/List/PrefabDataList")]
public class PrefabDataList : ScriptableObject
{
    [HideInInspector] public List<PrefabData> prefabDataList;

    public int Size()
    {
        return prefabDataList.Count;
    }

    public int GetPriority()
    {
        var sum = 0;
        foreach (var prefabData in prefabDataList) sum += prefabData.priority;
        return sum;
    }

    public GameObject GetRandomPrefab()
    {
        return prefabDataList[Random.Range(0, Size())].obj;
    }
}