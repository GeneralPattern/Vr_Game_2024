using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NavAgentBehavior))]
[RequireComponent(typeof(HealthBehavior))]
public class NavCreepController : MonoBehaviour, IDamageDealer
{
    public CreepData creepData;
    
    private NavAgentBehavior _agentBehavior;
    private HealthBehavior _health;

    private void Awake()
    {
        _health = GetComponent<HealthBehavior>();
        _agentBehavior = GetComponent<NavAgentBehavior>();
        _agentBehavior.SetSpeed(creepData.speed);
        _agentBehavior.SetRadius(creepData.radius);
        _agentBehavior.SetHeight(creepData.height);
        _health.maxHealth = creepData.health;
        _health.health = creepData.health;
    }
    
    public void StopMovement()
    {
        _agentBehavior.StopMovement();
    }

    private void OnCollisionEnter(Collision other)
    {
        var damagable = other.gameObject.GetComponent<IDamagable>();
        if (damagable != null) { DealDamage(damagable); }
    }

    public void DealDamage(IDamagable target, float amount)
    {
        
        target.TakeDamage(amount);
    }

    public void DealDamage(IDamagable target)
    {
        target.TakeDamage(this);
    }

    public float GetDamage()
    {
        return creepData.damage;
    }
}
