using UnityEngine;

public class CannonData : ScriptableObject
{
    public IntData ammo;
    public Vector3Data fireDirection;
    public FloatData fireForce, reloadTime, defaultReloadTime;
    public BoolData canFire;
    public GameObject cannonBallPrefab;
    public Transform cannonBallSpawnPoint;
    
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
