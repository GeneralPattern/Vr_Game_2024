using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CannonManager : MonoBehaviour
{
    private WaitForFixedUpdate _wffu;
    
    public UnityEvent onSuccessfulFire;
    
    [ReadOnly] public int ammo;
    public float fireForce;
    public Vector3 fireDirection;
    public Transform firePoint;
    public SocketMatchInteractor ammoSocket;
    private bool _respawnAvailable;

    private Coroutine _addForceCoroutine; 

    private void Awake()
    {
        _wffu = new WaitForFixedUpdate();
        ammo = 0;
        _addForceCoroutine = null;
    }

    public void Fire()
    {
        var ammoObj = ammoSocket.RemoveAndMoveSocketObject(firePoint.position, firePoint.rotation);
        if(ammoObj == null) {Debug.LogWarning("NO AMMO IN CANNON " + gameObject.name); return;}
        var ammoRb = ammoObj.GetComponent<Rigidbody>();
        
        if (ammo < 1) {Debug.LogWarning("Ammo Count: " + ammo); return;}
        if (_addForceCoroutine != null){ return;}
        onSuccessfulFire.Invoke();
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
        ammoRb.AddForce(fireDirection * fireForce, ForceMode.Impulse);
        _addForceCoroutine = null; 
    }

    public void IncrementAmmo()
    {
        ammo++;
    }

    public void DecrementAmmo()
    {
        ammo--;
    }
}