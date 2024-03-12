using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
// using static ZpTools.UtilityFunctions;
using Random = UnityEngine.Random;


public class AdvancedSpawnManager : MonoBehaviour
// public class AdvancedSpawnManager : MonoBehaviour, INeedButton
{
    /*
    public UnityEvent onSpawn, onSpawningComplete;

    public SpawnerData spawnerData;
    public bool usePriority, spawnOnStart, randomizeSpawnRate;

    [System.Serializable]
    public class Spawner
    {
        public string spawnerID;
        public Transform spawnLocation;
        private int _currentSpawnCount;
        public int activeSpawnLimit;

        public int GetAliveCount() { return _currentSpawnCount; }
        public void IncrementCount() { _currentSpawnCount++; }
        public void DecrementCount() { _currentSpawnCount--; }
    }

#pragma warning disable CS0414 // Field is assigned but its value is never used
    [SerializeField] [ReadOnly] private int activeCount;
#pragma warning restore CS0414 // Field is assigned but its value is never used
    public List<Spawner> spawners = new();

    public float poolCreationDelay = 1.0f, spawnDelay = 1.0f, spawnRateMin = 1.0f, spawnRateMax = 1.0f;
    public int numToSpawn = 10;

    private int _poolSize;

    private int poolSize
    {
        get
        {
            int totalPoolSize = 0;
            foreach (var spawner in spawners)
            {
                totalPoolSize += spawner.activeSpawnLimit;
            }

            return totalPoolSize;
        }
    }

    private List<GameObject> _pooledObjects;

    private float _spawnRateStatic;

    private float spawnRate
    {
        get
        {
            var value = Random.Range(spawnRateMin, spawnRateMax);
            return value;
        }
    }

    private WaitForSeconds _waitForSpawnRate, _waitForSpawnDelay, _waitForPoolDelay, _locationCheckDelay;
    private WaitForFixedUpdate _wffu;
    private Coroutine _lateStartRoutine, _delaySpawnRoutine, _spawnRoutine, _poolCreationRoutine;

    private PrefabDataList _prefabSet;

    private int spawnedCount
    {
        get => spawnerData.activeCount.value;
        set => spawnerData.activeCount.UpdateValue(value);
    }

    private void Awake()
    {
        _wffu = new WaitForFixedUpdate();

        _poolSize = poolSize;
        _waitForPoolDelay = new WaitForSeconds(poolCreationDelay);

        _waitForSpawnDelay = new WaitForSeconds(spawnDelay);
        _locationCheckDelay = new WaitForSeconds(1.0f);

        _spawnRateStatic = spawnRate;
        _waitForSpawnRate = new WaitForSeconds(_spawnRateStatic);
        _waitForSpawnRate = randomizeSpawnRate ? new WaitForSeconds(spawnRate) : _waitForSpawnRate;

        if (!spawnerData)
        {
            Debug.LogError("SpawnerData not found in " + name);
            return;
        }

        spawnerData.ResetSpawner();
        _prefabSet = spawnerData.prefabDataList;

        _poolCreationRoutine ??= StartCoroutine(DelayPoolCreation());
    }

    private IEnumerator DelayPoolCreation()
    {
        yield return _waitForPoolDelay;
        ProcessPool();
    }

    private void ProcessPool()
    {
        _pooledObjects ??= new List<GameObject>();
        int iterationCount = _poolSize - _pooledObjects.Count;
        if (iterationCount <= 0) return;
        
        int totalPriority = _prefabSet.GetPriority();

        for (int i = 0; i < iterationCount; i++)
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

    private void AddToPool(GameObject obj)
    {
        var spawnBehavior = obj.GetComponent<AdvancedSpawnedObjectBehavior>();
        if (spawnBehavior == null) obj.AddComponent<AdvancedSpawnedObjectBehavior>();

        _pooledObjects.Add(obj);
        obj.SetActive(false);
    }

    public void SetSpawnDelay(float newDelay)
    {
        newDelay = ToleranceCheck(spawnDelay, newDelay);
        if (newDelay < 0) return;
        spawnDelay = newDelay;
        _waitForSpawnDelay = new WaitForSeconds(spawnDelay);
    }
    
    private void Start()
    {
        if (spawnOnStart)
        {
            _lateStartRoutine ??= StartCoroutine(LateStartSpawn());
        }
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

    public void StartSpawn(int amount)
    {
        if (_spawnRoutine != null) return;
        numToSpawn = (amount > 0) ? amount : numToSpawn;
        StartSpawn();
    }

    public void StartSpawn()
    {
        if (_spawnRoutine != null) return;
        numToSpawn = numToSpawn > 0 ? numToSpawn : 1;
        if (spawnedCount > 0) { spawnerData.ResetSpawner(); spawnedCount = 0;}
        _delaySpawnRoutine ??= StartCoroutine(DelaySpawn());
    }

    private IEnumerator DelaySpawn()
    {
        yield return _wffu;
        yield return _waitForSpawnDelay;
        _spawnRoutine ??= StartCoroutine(Spawn());
    }
    
    private void Spawn()
    {
        
        while (spawnedCount < numToSpawn)
        {
            Debug.Log($"Spawning... Count: {spawnedCount} NumToSpawn: {numToSpawn} PoolSize: {_poolSize} PooledObjects: {_pooledObjects.Count} spawners: {spawners.Count} spawnRate: {_spawnRateStatic}");
            GameObject spawnObj = FetchFromPool();
            _poolSize = _pooledObjects.Count;
            
            if (!spawnObj)
            {
                _poolSize++;
                ProcessPool();
                continue;
            }
            
            Transform objTransform = spawnObj.transform;
            AdvancedSpawnedObjectBehavior objBehavior = spawnObj.GetComponent<AdvancedSpawnedObjectBehavior>();
            if (objBehavior == null) Debug.LogError($"No SpawnObjectBehavior found on {spawnObj} in ProcessSpawnedObject Method");
            var rb = spawnObj.GetComponent<Rigidbody>();
        
            objBehavior.spawnManager = this;
            objBehavior.spawned = true;
            
            if (rb)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            Debug.Log($"Retrieved Spawner: {spawner.spawnerID} with... Count: {spawner.GetAliveCount()} Limit: {spawner.activeSpawnLimit}"); 
            Transform spawnLocation = spawner.spawnLocation;
            
            objTransform.position = spawnLocation.position;
            objTransform.rotation = spawnLocation.rotation;
        
            spawnObj.SetActive(true);
            onSpawn.Invoke();

        }
        onSpawningComplete.Invoke();
        
        _lateStartRoutine = null;
        _delaySpawnRoutine = null;
        _spawnRoutine = null;
    }

    private GameObject FetchFromPool() { return FetchFromList(_pooledObjects, obj => !obj.activeInHierarchy); }

    private Spawner RandomlyFetchFromAllOpenSpawners()
    {
        var availableSpawners = new List<Spawner>();
        foreach (var spawner in spawners)
        {
            if (spawner.GetAliveCount() < spawner.activeSpawnLimit)
            {
                availableSpawners.Add(spawner);
            }
        }
        if (availableSpawners.Count == 0) return null;
        return availableSpawners[Random.Range(0, availableSpawners.Count)];
    }

    private IEnumerator GetSpawner()
    {
        Spawner spawner = null;
        while (spawner == null)
        {
            spawner = FetchSpawner();
            if (spawner == null)
            {
                Debug.Log("No Spawner Found... Waiting for next check");
                yield return _locationCheckDelay;
            }
            else
            {
                spawner.IncrementCount();
            }
        }
    }
    
    public void NotifyOfSpawn(string spawnerID)
    {
        Debug.Log($"Notified of Spawn: {spawnerID}");
        spawnedCount++; 
       
        for (int i = 0; i < spawners.Count; i++)
        {
            if (spawners[i].spawnerID == spawnerID)
            {
                var spawner = spawners[i];
                spawner.IncrementCount();
                spawners[i] = spawner;
                break;
            }
        }
    }
    
    public void NotifyOfDeath(string spawnerID)
    {
        Debug.Log($"Notified of Death: {spawnerID}");
        for (int i = 0; i < spawners.Count; i++)
        {
            if (spawners[i].spawnerID == spawnerID)
            {
                var spawner = spawners[i];
                spawner.DecrementCount();
                spawners[i] = spawner;
                break;
            }
        }
    }
    
    public List<(System.Action, string)> GetButtonActions()
    {
        return new List<(System.Action, string)> { (() => StartSpawn(numToSpawn), "Spawn") };
    }
}

namespace ZpTools
{
    public static class UtilityFunctions
    {
        public static float ToleranceCheck(float value, float newValue, float tolerance = 0.1f)
        {
            return System.Math.Abs(value - newValue) < tolerance ? value : newValue;
        }
        
        public static T FetchFromList<T>(List<T> listToProcess, System.Func<T, bool> condition)
        {
            if (listToProcess == null || listToProcess.Count == 0) return default;
            foreach (var obj in listToProcess)
            {
                if (condition(obj)) return obj;
            }
            return default;
        }
    }
*/
}