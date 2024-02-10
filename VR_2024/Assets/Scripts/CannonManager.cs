using System;
using System.Collections;
using UnityEngine;

public class CannonManager : MonoBehaviour
{
    private WaitForFixedUpdate _wffu; 
    
    public bool canFire;
    [ReadOnly] public int ammo;
    [ReadOnly] public float reloadTime = 0.0f;
    public float defaultReloadTime, fireForce;
    public Vector3 fireDirection;
    public Transform firePoint;
    public GameObject ammoRespawn;
    public SocketMatchInteractor ammoSocket;
    private SpawnManager _ammoSpawner;
    private bool _respawnAvailable;

    private Coroutine _addForceCoroutine; 

    private void Awake()
    {
        _wffu = new WaitForFixedUpdate();
        ammo = 0;
        _ammoSpawner = ammoRespawn.GetComponent<SpawnManager>();
        _respawnAvailable = true;
        _addForceCoroutine = null;
    }

    public void Fire()
    {
        var ammoObj = ammoSocket.RemoveAndMoveSocketObject(firePoint.position, firePoint.rotation);
        if(ammoObj == null) {Debug.LogWarning("NO AMMO IN CANNON " + gameObject.name); return;}
        var ammoRb = ammoObj.GetComponent<Rigidbody>();
        // if (!canFire || ammo < 1) return; // canFire will be implemented with reload mechanic
        
        if (ammo < 1) {Debug.LogWarning("Ammo Count: " + ammo); return;}
        if (_addForceCoroutine != null){ return;}
        Debug.Log(fireDirection + "*" + fireForce + " = " + fireDirection * fireForce);
        _addForceCoroutine = StartCoroutine(AddForceToAmmo(ammoRb));
        DecrementAmmo();
    }
    
    private IEnumerator AddForceToAmmo(Rigidbody ammoRb)
    {
        ammoRb.isKinematic = false;
        yield return _wffu;
        yield return _wffu;
        yield return _wffu;
        yield return null;
        Debug.Log("Adding force to ammo");
        ammoRb.AddForce(fireDirection * fireForce, ForceMode.Impulse);
        _addForceCoroutine = null; 
    }
    
    private IEnumerator RespawnAmmo()
    {
        yield return _wffu;
        _ammoSpawner.StartSpawn(1);
    }

    public void IncrementAmmo()
    {
        ammo++;
    }

    public void DecrementAmmo()
    {
        ammo--;
    }
    
    public void SetSpawnAvailability(bool value)
    {
        _respawnAvailable = value;
    }
    
    public void Reload()
    {
        // if (!canFire)
        // {
        //     reloadTime.value -= Time.deltaTime;
        //     if (reloadTime <= 0)
        //     {
        //         canFire = true;
        //         reloadTime = defaultReloadTime.value;
        //     }
        // }
    }
}