using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour, INeedButton
{
    public UnityEvent onSpawn;

    public SpawnerData spawnerData;

    public bool usePriority, spawnOnStart, setTransformToThisTransform;

    [SerializeField] [ReadOnly] private int activeCount;
    
    public int numToSpawn = 10, poolSize = 10;
    public float poolCreationDelay = 1.0f, spawnDelay = 1.0f, spawnRate = 0.3f;
    
    private int _spawnedCount;

    private WaitForSeconds _waitForSpawnRate, _waitForSpawnDelay, _waitForPoolDelay;
    private WaitForFixedUpdate _wffu;
    private PrefabDataList _prefabSet;
    private List<GameObject> _pooledObjects;

    public void SetSpawnDelay(float newDelay) 
    { 
        spawnDelay = newDelay;
        _waitForSpawnDelay = new WaitForSeconds(spawnDelay);
    }
    public float GetSpawnDelay() { return spawnDelay; }
    
    public void SetPoolSize(int newSize) { poolSize = newSize; }
    public int GetPoolSize() { return poolSize; }

    public void SetSpawnRate(float newRate)
    {
        spawnRate = newRate;
        _waitForSpawnRate = new WaitForSeconds(spawnRate);
    }
    public float GetSpawnRate() { return spawnRate; }
    
    private void Awake()
    {
        activeCount = 0;
        _waitForSpawnRate = new WaitForSeconds(spawnRate);
        _waitForSpawnDelay = new WaitForSeconds(spawnDelay);
        _waitForPoolDelay = new WaitForSeconds(poolCreationDelay);
        _wffu = new WaitForFixedUpdate();
        spawnerData.ResetSpawner();
        _prefabSet = spawnerData.prefabDataList;
        StartCoroutine(DelayPoolCreation());
    }

    private void Start()
    {
        if (spawnOnStart) StartSpawn(numToSpawn);
    }

    public void StartSpawn(int amount)
    {
        numToSpawn = amount;
        StartCoroutine(DelaySpawn());
    }

    private IEnumerator DelayPoolCreation()
    {
        yield return _waitForPoolDelay;
        CreatePool();
    }

    private IEnumerator DelaySpawn()
    {
        yield return _wffu;
        yield return _waitForSpawnDelay;
        StartCoroutine(Spawner());
    }
    
    private void CreatePool()
    {
        _pooledObjects = new List<GameObject>();
        
        int totalPriority = _prefabSet.GetPriority();
        
        for (int i = 0; i < poolSize; i++)
        {
            int randomNumber = Random.Range(0, totalPriority);
            int sum = 0;
            foreach (var prefabData in _prefabSet.prefabDataList)
            {
                sum += prefabData.priority;
                if (randomNumber < sum || !usePriority)
                {
                    GameObject obj = Instantiate(prefabData.obj);
                    AddToPool(obj);
                    break;
                }
            }
        }
    }

    private IEnumerator Spawner()
    {
        _spawnedCount = 0;
        while (_spawnedCount < numToSpawn)
        {
            activeCount =  spawnerData.GetAliveCount();
            var spawnObj = FetchFromPool();
            if (spawnObj) Spawn(spawnObj);
            else IncreasePoolAndSpawn();
            yield return _waitForSpawnRate;
        }
    }
    
    private GameObject FetchFromPool()
    {
        if (_pooledObjects.Count == 0) return null;
        foreach (var obj in _pooledObjects)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        return null;
    }
    
    protected virtual void Spawn(GameObject obj)
    {
        if (!obj) return;

        var rb = obj.GetComponent<Rigidbody>();
        var spawnObj = obj.GetComponent<SpawnedObjectBehavior>();

        if (rb) rb.velocity = Vector3.zero;

        if (spawnObj)
        {
            spawnObj.SetSpawnManager(this);
            if (setTransformToThisTransform)
            {
                if (spawnObj.GetSpawnPosition() == Vector3.zero) spawnObj.SetSpawnPosition(transform.position);
                if (spawnObj.GetSpawnRotation() == Quaternion.identity) spawnObj.SetSpawnRotation(Quaternion.identity);
            }

            obj.transform.position = spawnObj.GetSpawnPosition();
            obj.transform.rotation = spawnObj.GetSpawnRotation();
        }
        else
        {
            obj.transform.position = transform.position;
            obj.transform.rotation = Quaternion.identity;
        }

        obj.SetActive(true);
        _spawnedCount++;
        spawnerData.IncrementActiveInstancesCount();
        onSpawn.Invoke();
    }

    protected void IncreasePoolAndSpawn()
    {
        int totalPriority = spawnerData.prefabDataList.GetPriority();
        int randomNumber = Random.Range(0, totalPriority);
        int sum = 0;
        foreach (var prefabData in _prefabSet.prefabDataList)
        {
            sum += prefabData.priority;
            if (randomNumber < sum || !usePriority)
            {
                var prefab = FetchPrefab(prefabData);
                AddToPool(prefab);
                Spawn(prefab);
                break;
            }
        }
    }
    
    protected virtual GameObject FetchPrefab(PrefabData data)
    {
        var obj = data.obj;
        return obj;
    }
    
    protected virtual void AddToPool(GameObject obj)
    {
        var spawnObj = obj.GetComponent<SpawnedObjectBehavior>();
        if(!spawnObj) obj.AddComponent<SpawnedObjectBehavior>();
        
        _pooledObjects.Add(obj);
        obj.SetActive(false);
    }
    
    public void ButtonAction()
    {
       StartSpawn(numToSpawn);
    }

    public string GetButtonName()
    {
        return "Spawn";
    }
}
