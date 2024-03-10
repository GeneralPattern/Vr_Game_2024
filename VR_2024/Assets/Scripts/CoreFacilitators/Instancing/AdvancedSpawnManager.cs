using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AdvancedSpawnManager : MonoBehaviour, INeedButton
{
    [System.Serializable]
    public struct Spawner
    {
        public string spawnerID;
        public Transform spawnLocation;
        public int spawnCount;
        public int locationSpawnLimit;
        public int GetAliveCount() { return spawnCount; }
        public void IncrementCount() { spawnCount++; }
        public void DecrementCount() { spawnCount--; }
    }

#pragma warning disable CS0414 // Field is assigned but its value is never used
    [SerializeField] [ReadOnly] private int activeCount;
#pragma warning restore CS0414 // Field is assigned but its value is never used
    
    public List<Spawner> spawners = new();
    public UnityEvent onSpawn, onSpawningComplete;
    public SpawnerData spawnerData;
    public bool usePriority, spawnOnStart;
    public int numToSpawn = 10;
    public float poolCreationDelay = 1.0f, spawnDelay = 1.0f, spawnRateMin = 1.0f, spawnRateMax = 1.0f;

    private int spawnedCount
    {
        get 
        {
            int totalSpawnCount = 0;
            foreach (var spawner in spawners)
            {
                totalSpawnCount += spawner.GetAliveCount();
            }
            return totalSpawnCount;
        }
        set => spawnerData.activeCount.UpdateValue(value);
    }
    
    private bool _overrideLocation;

    private WaitForSeconds _waitForSpawnRate, _waitForSpawnDelay, _waitForPoolDelay;
    private WaitForFixedUpdate _wffu;
    private PrefabDataList _prefabSet;
    private List<GameObject> _pooledObjects;
    
    private Coroutine _spawnRoutine, _poolCreationRoutine;

    public void SetSpawnDelay(float newDelay) 
    { 
        if (spawnDelay == newDelay) return;
        if (newDelay < 0) return;
        spawnDelay = newDelay;
        _waitForSpawnDelay = new WaitForSeconds(spawnDelay);
    }
    
    public void SetSpawnRate(float newMin, float newMax)
    {
        spawnRateMin = newMin;
        spawnRateMax = newMax;
        _waitForSpawnRate = new WaitForSeconds(spawnRate);
    }

    private float spawnRate
    {
        get
        {
            var value = Random.Range(spawnRateMin, spawnRateMax);
            _waitForSpawnRate = new WaitForSeconds(value);
            return value;
        }
    }
    
    private int _poolSize;
    private int poolSize
    {
        get
        {
            int totalPoolSize = 0;
            foreach (var spawner in spawners)
            {
                totalPoolSize += spawner.locationSpawnLimit;
            }
            return totalPoolSize;
        }
    }
    
    private void Awake()
    {
        _poolSize = poolSize;
        
        _waitForSpawnRate = new WaitForSeconds(spawnRate);
        _waitForSpawnDelay = new WaitForSeconds(spawnDelay);
        _waitForPoolDelay = new WaitForSeconds(poolCreationDelay);
        _wffu = new WaitForFixedUpdate();
        
        if (!spawnerData) { Debug.LogError("SpawnerData not found in " + name); return; }
        spawnerData.ResetSpawner();
        _prefabSet = spawnerData.prefabDataList;
        _poolCreationRoutine = StartCoroutine(DelayPoolCreation());
    }
    
    private void Start()
    {
        if (spawnOnStart) StartCoroutine(LateStartSpawn());
    }
    
    private IEnumerator LateStartSpawn()
    {
        var count = 0;
        while (count < 10)
        {
            count++;
            yield return _wffu;
        }
        StartSpawn();
    }

    private IEnumerator DelayPoolCreation()
    {
        yield return _waitForPoolDelay;
        ProcessPool();
    }

    private IEnumerator DelaySpawn()
    {
        yield return _wffu;
        yield return _waitForSpawnDelay;
        StartCoroutine(SpawnRoutine());
    }
    
    private void ProcessPool(bool spawn = false)
    {
        _pooledObjects ??= new List<GameObject>();
        
        int totalPriority = _prefabSet.GetPriority();
        
        for (int i = 0; i < _poolSize; i++)
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
                    if (spawn) Spawn(obj);
                    break;
                }
            }
        }
    }

    public void StartSpawn(int amount)
    {
        if (_spawnRoutine != null && _poolCreationRoutine != null) return;
        numToSpawn = (amount > 0) ? amount : numToSpawn;
        StartSpawn();
    }

    public void StartSpawn()
    {
        if (_spawnRoutine != null && _poolCreationRoutine != null) return;
        numToSpawn = (numToSpawn > 0) ? numToSpawn : 1;
        if (spawnedCount > 0) spawnerData.ResetSpawner();
        _spawnRoutine = StartCoroutine(DelaySpawn());
    }

    private IEnumerator SpawnRoutine()
    {
        spawnedCount = 0;
        while (spawnedCount < numToSpawn)
        {
            InternalSpawnRoutine();
            yield return _waitForSpawnRate;
        }
        onSpawningComplete.Invoke();
        _spawnRoutine = null;
    }
    
    protected virtual void InternalSpawnRoutine()
    {
        spawnedCount =  spawnerData.GetAliveCount();
        var spawnObj = FetchFromPool();
        if (spawnObj) Spawn(spawnObj);
        else IncreasePoolAndSpawn();
    }
    
    private GameObject FetchFromPool()
    {
        if (_pooledObjects == null) return null;
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
        if (!obj) {Debug.LogError("There is no object provided to spawn, killing process"); return;}
        var spawnObj = obj.GetComponent<SpawnedObjectBehavior>();
        
        var rb = obj.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        
        if (spawnObj == null) Debug.LogError($"No SpawnObjectBehavior found on {obj} in ProcessSpawnedObject Method");
        
        ProcessSpawnedObject(obj, spawnObj);
        obj.SetActive(true);

        spawnedCount++;
        onSpawn.Invoke();
    }
    
    protected virtual void ProcessSpawnedObject(GameObject obj, SpawnedObjectBehavior spawnObj, Transform spawnLocation = null)
    {
        /*
        if (spawnLocation)
        {
            if (_overrideLocation)
            {
                spawnObj.spawnPosition = spawnLocation.position;
                spawnObj.spawnRotation = spawnLocation.rotation;
            }
            else
            {
                var potentialSpawnPos = spawnObj.spawnPosition;
                var potentialSpawnRot = spawnObj.spawnRotation;
            
                spawnObj.spawnPosition = potentialSpawnPos != Vector3.zero ? potentialSpawnPos : spawnLocation.position;
                spawnObj.spawnRotation = potentialSpawnRot != Quaternion.identity ? potentialSpawnRot : spawnLocation.rotation;
            }
            
            obj.transform.position = spawnObj.spawnPosition;
            obj.transform.rotation = spawnObj.spawnRotation;
        }
        else
        {
            var objTransform = obj.transform;
            objTransform.position = transform.position;
            objTransform.rotation = Quaternion.identity;
        }
        */
    }

    protected void IncreasePoolAndSpawn()
    {   
        _poolSize++;
        ProcessPool(true);
    }
    
    protected virtual void AddToPool(GameObject obj)
    {
        var spawnBehavior = obj.GetComponent<AdvancedSpawnedObjectBehavior>();
        if (spawnBehavior == null) obj.AddComponent<AdvancedSpawnedObjectBehavior>();
        spawnBehavior.spawnManager = this;
        
        _pooledObjects.Add(obj);
        obj.SetActive(false);
    }

    public List<(System.Action, string)> GetButtonActions()
    {
        return new List<(System.Action, string)> { (() => StartSpawn(numToSpawn), "Spawn") };
    }
}