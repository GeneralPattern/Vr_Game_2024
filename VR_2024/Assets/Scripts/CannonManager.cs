using UnityEngine;

public class CannonManager : MonoBehaviour
{
    [HideInInspector] public CannonData cannonData;
    public Vector3 fireDirection;
    public Transform firePoint;
    public GameObject cannonBallPrefab;
    
    private void Start()
    {
        cannonData = ScriptableObject.CreateInstance<CannonData>();
        cannonData.ammo.value = 0;
        cannonData.fireDirection.value = fireDirection;
        cannonData.fireForce.value = 1000;
        cannonData.reloadTime.value = 1;
        cannonData.defaultReloadTime.value = 1;
        cannonData.canFire.value = true;
        cannonData.cannonBallPrefab = cannonBallPrefab;
        cannonData.cannonBallSpawnPoint = firePoint;
    }
    
    public void Fire()
    {
        cannonData.Fire();
    }
    
    public void Reload()
    {
        cannonData.Reload();
    }
    
    public void IncrementAmmo()
    {
        cannonData.IncrementAmmo();
    }
}
