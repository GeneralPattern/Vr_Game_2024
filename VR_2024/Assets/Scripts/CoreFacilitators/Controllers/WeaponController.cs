using UnityEngine;
using UnityEngine.Events;

public class WeaponController : MonoBehaviour, IDamagable, IDamageDealer
{
    public UnityEvent onDurabilityDepleted;
    
    [SerializeField] [ReadOnly] private float durability = 100;
    [SerializeField] [ReadOnly] private float damage;
    private FloatData _damage;
    
    public void SetDurability(float newDurability) { durability = newDurability; }
    public float GetDurability() { return durability; }
    
    public void SetDamage(float newDamage) { _damage.SetValue(newDamage); damage = newDamage; }
    public float GetDamage() { return damage; }
    
    public void Start()
    {
        _damage = ScriptableObject.CreateInstance<FloatData>();
        damage = (_damage.value > 0) ? _damage.value : 1;
    }

    private void OnCollisionEnter(Collision other)
    {
        var damagable = other.gameObject.GetComponent<IDamagable>();
        if (damagable != null) { DealDamage(damagable); }
    }

    public void TakeDamage(float amount)
    {
        durability -= amount;
        if (durability <= 0)
        {
            onDurabilityDepleted.Invoke();
        }
    }

    public void TakeDamage(IDamageDealer dealer)
    {
    }
    
    public void DealDamage(IDamagable target)
    {
        target.TakeDamage(damage);
    }

    public void DealDamage(IDamagable target, float amount)
    {
        target.TakeDamage(amount);
    }
}
