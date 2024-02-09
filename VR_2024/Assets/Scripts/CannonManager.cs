using System;
using System.Collections;
using UnityEngine;

public class CannonManager : MonoBehaviour
{
    [HideInInspector] public CannonData cannonData;
    public Vector3 fireDirection;
    public Transform firePoint;
    public GameObject ammoSocketManager;
    public GameObject ammoRespawn;
    
    private SocketMatchInteractor _ammoSocket;
    private SpawnManager _ammoSpawner;
    private bool _respawnAvailable;

    private void Awake()
    {
        cannonData = ScriptableObject.CreateInstance<CannonData>();
        cannonData.ammo = 0;
        cannonData.outputSpawnPoint = firePoint;
        _ammoSocket = ammoSocketManager.GetComponent<SocketMatchInteractor>();
        _ammoSpawner = ammoRespawn.GetComponent<SpawnManager>();
        _respawnAvailable = true;
    }

    // private void Start()
    // {
    //     StartCoroutine(waitForSetup());
    // }
    //
    // private IEnumerator waitForSetup()
    // {
    //     yield return new WaitForSeconds(1);
    //     RespawnAmmo();
    // }

    public void Fire()
    {
        Debug.Log(_ammoSocket.GetSocketedObject());
        cannonData.Fire(_ammoSocket.GetSocketedObject());
    }

    public void Reload()
    {
        cannonData.Reload();
    }

    public void IncrementAmmo()
    {
        cannonData.IncrementAmmo();
    }
    
    public void RespawnAmmo()
    {
        if (!_respawnAvailable) return;
        _ammoSpawner.StartSpawn(1);
        _respawnAvailable = false;
    }
    
    public void SetSpawnAvailability(bool value)
    {
        _respawnAvailable = value;
    }
}