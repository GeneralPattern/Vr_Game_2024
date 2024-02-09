using UnityEngine;

[CreateAssetMenu]
public class CannonData : ScriptableObject
{
    public int ammo;
    public Vector3 fireDirection;
    public float fireForce, reloadTime, defaultReloadTime;
    public bool canFire;
    public Transform outputSpawnPoint;
    
    // [HideInInspector] public IntData _ammo;
    // [HideInInspector] public Vector3Data _fireDirection;
    // [HideInInspector] public FloatData _fireForce, _reloadTime, _defaultReloadTime;
    // [HideInInspector] public BoolData _canFire;
    // [HideInInspector] public Transform _outputSpawnPoint;

    private void Awake()
    {
        // if (ammo == null) ammo = CreateInstance<IntData>();
        // if (fireDirection == null) fireDirection = CreateInstance<Vector3Data>();
        // if (fireForce == null) fireForce = CreateInstance<FloatData>();
        // if (reloadTime == null) reloadTime = CreateInstance<FloatData>();
        // if (defaultReloadTime == null) defaultReloadTime = CreateInstance<FloatData>();
        // if (canFire == null) canFire = CreateInstance<BoolData>();
        
    }

    public void Fire(GameObject ammoObj)
    {
        if(ammoObj == null) return;
        // if (!canFire || ammo < 1) return;
        ammoObj.transform.position = outputSpawnPoint.position;
        ammoObj.transform.rotation = outputSpawnPoint.rotation;
        ammoObj.GetComponent<Rigidbody>().AddForce(fireDirection * fireForce, ForceMode.Impulse);
        ammo--;
    }
    
    public void Reload()
    {
        // if (!canFire.value)
        // {
        //     reloadTime.value -= Time.deltaTime;
        //     if (reloadTime.value <= 0)
        //     {
        //         canFire.value = true;
        //         reloadTime.value = defaultReloadTime.value;
        //     }
        // }
    }
    
    public void IncrementAmmo()
    {
        // ammo.IncrementValue();
    }
}
