using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour, INeedButton
{
    public UnityEvent onSpawn;

    public SpawnerData spawnerData;

    public bool usePriority, spawnOnStart;
    public Transform spawnLocation;

    // ReSharper disable once NotAccessedField.Local
    [SerializeField] [ReadOnly] private int activeCount;
    
    public int numToSpawn = 10, poolSize = 10;
    public float poolCreationDelay = 1.0f, spawnDelay = 1.0f, spawnRate = 0.3f;

    private int spawnedCount
    {
        get => spawnerData.GetAliveCount();
        set => spawnerData.activeCount.UpdateValue(value);
    }
    
    private bool _overrideLocation;

    private WaitForSeconds _waitForSpawnRate, _waitForSpawnDelay, _waitForPoolDelay;
    private WaitForFixedUpdate _wffu;
    private PrefabDataList _prefabSet;
    private List<GameObject> _pooledObjects;
    
    private Coroutine _spawnRoutine;
    
    public void SetSpawnDelay(float newDelay) 
    { 
        spawnDelay = newDelay;
        _waitForSpawnDelay = new WaitForSeconds(spawnDelay);
    }
    public float GetSpawnDelay() { return spawnDelay; }

    public void SetSpawnRate(float newRate)
    {
        spawnRate = newRate;
        _waitForSpawnRate = new WaitForSeconds(spawnRate);
    }
    
    private void Awake()
    {
        spawnedCount = 0;
        _waitForSpawnRate = new WaitForSeconds(spawnRate);
        _waitForSpawnDelay = new WaitForSeconds(spawnDelay);
        _waitForPoolDelay = new WaitForSeconds(poolCreationDelay);
        _wffu = new WaitForFixedUpdate();
        if (!spawnerData) { Debug.LogError("SpawnerData not found in " + name); return; }
        spawnerData.ResetSpawner();
        _prefabSet = spawnerData.prefabDataList;
        StartCoroutine(DelayPoolCreation());
        if(!spawnLocation) spawnLocation = transform;
    }

    protected virtual void Start()
    {
        if (spawnOnStart)
        {
            var originalSpawnDelay = spawnDelay;
            spawnDelay += poolCreationDelay;
            StartSpawn(numToSpawn);
            spawnDelay = originalSpawnDelay;
        }
    }

    public void StartSpawn(int amount)
    {
        if (_spawnRoutine != null) return;
        numToSpawn = (amount > 0) ? amount : numToSpawn;
        StartSpawn();
    }

    public void StartSpawn()
    {
        if (_spawnRoutine != null) return;
        numToSpawn = (numToSpawn > 0) ? numToSpawn : 1;
        if (spawnedCount > 0) spawnerData.ResetSpawner();
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
        StartCoroutine(SpawnRoutine());
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

    private IEnumerator SpawnRoutine()
    {
        spawnedCount = 0;
        while (spawnedCount < numToSpawn)
        {
            InternalSpawnRoutine();
            yield return _waitForSpawnRate;
        }
    }
    
    protected virtual void InternalSpawnRoutine()
    {
        spawnedCount =  spawnerData.GetAliveCount();
        var spawnObj = FetchFromPool();
        if (spawnObj) Spawn(spawnObj);
        else IncreasePoolAndSpawn();
    }
    
    protected virtual void Spawn(GameObject obj)
    {
        if (!obj) return;

        var rb = obj.GetComponent<Rigidbody>();
        var spawnObj = obj.GetComponent<SpawnedObjectBehavior>();

        if (rb) rb.velocity = Vector3.zero;
        
        ProcessSpawnedObject(spawnObj);

        obj.SetActive(true);
        spawnedCount++;
        onSpawn.Invoke();
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
    
    protected virtual void ProcessSpawnedObject(SpawnedObjectBehavior spawnObj)
    {
        if (spawnObj && spawnLocation)
        {
            spawnObj.SetSpawnManager(this);
            if (_overrideLocation)
            {
                spawnObj.SetSpawnPosition(spawnLocation.position);
                spawnObj.SetSpawnRotation(spawnLocation.rotation);
            }
            else
            {
                var potentialSpawnPos = spawnObj.GetSpawnPosition();
                var potentialSpawnRot = spawnObj.GetSpawnRotation();
            
                spawnObj.SetSpawnPosition(potentialSpawnPos != Vector3.zero ? potentialSpawnPos : spawnLocation.position);
                spawnObj.SetSpawnRotation(potentialSpawnRot == Quaternion.identity ? potentialSpawnRot : spawnLocation.rotation);
            }
        }
        else
        {
            var objTransform = spawnObj.transform;
            objTransform.position = transform.position;
            objTransform.rotation = Quaternion.identity;
        }
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

    public List<(System.Action, string)> GetButtonActions()
    {
        return new List<(System.Action, string)> { (() => StartSpawn(numToSpawn), "Spawn") };
    }
}
