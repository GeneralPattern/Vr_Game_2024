using System;
using UnityEngine;

public class CannonData : ScriptableObject
{
    public IntData ammo;
    public Vector3Data fireDirection;
    public FloatData fireForce, reloadTime, defaultReloadTime;
    public BoolData canFire;
    public GameObject cannonBallPrefab;
    public Transform cannonBallSpawnPoint;

    private void Awake()
    {
        if (ammo == null) ammo = CreateInstance<IntData>();
        if (fireDirection == null) fireDirection = CreateInstance<Vector3Data>();
        if (fireForce == null) fireForce = CreateInstance<FloatData>();
        if (reloadTime == null) reloadTime = CreateInstance<FloatData>();
        if (defaultReloadTime == null) defaultReloadTime = CreateInstance<FloatData>();
        if (canFire == null) canFire = CreateInstance<BoolData>();
        
    }

    public void Fire()
    {
        if (ammo.value > 0 && canFire.value)
        {
            var cannonBall = Instantiate(cannonBallPrefab, cannonBallSpawnPoint.position, Quaternion.identity);
            cannonBall.GetComponent<Rigidbody>().AddForce(fireDirection.value * fireForce.value);
            ammo.DecrementValue();
            canFire.value = false;
        }
    }
    
    public void Reload()
    {
        if (!canFire.value)
        {
            reloadTime.value -= Time.deltaTime;
            if (reloadTime.value <= 0)
            {
                canFire.value = true;
                reloadTime.value = defaultReloadTime.value;
            }
        }
    }
    
    public void IncrementAmmo()
    {
        ammo.IncrementValue();
    }
}
