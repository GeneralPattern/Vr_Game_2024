using UnityEngine;
using UnityEngine.Events;

public class HealthBehavior : MonoBehaviour, IDamagable
{
    [System.Serializable]
    private struct HealthEvents
    {
        public float atHealth;
        public UnityEvent healthEvent;
    }
    
    [SerializeField] private HealthEvents[] healthEvents;
    private bool _performHealthEvents;
    
    public UnityEvent onHealthGained, onHealthLost, onHealthDepleted;
    
    [SerializeField] [ReadOnly] private float currentHealth;
    public float maxHealth;

    public float health
    {
        get => currentHealth;
        set => currentHealth = value;
    }

    private void Awake()
    {
        _performHealthEvents = healthEvents.Length > 0;
    }

    private void Start()
    {
        if (maxHealth <= 0) maxHealth = 1;
        health = maxHealth;
    }
    
    private void CheckHealthEvents()
    {
        foreach (var healthEvent in healthEvents)
        {
            if (health == healthEvent.atHealth) healthEvent.healthEvent.Invoke();
        }
    }
    
    public void SetHealth(float newHealth)
    {
        health = newHealth;
        if (health > maxHealth) health = maxHealth;
        if (health < 0) health = 0;
        if (_performHealthEvents) CheckHealthEvents();
    }
    
    public void SetMaxHealth(float newHealth)
    {
        maxHealth = newHealth;
    }

    public void LoseHealth(float amount)
    {
        health -= amount;
        if (health <= 0) onHealthDepleted.Invoke();
        else onHealthLost.Invoke();
        if (_performHealthEvents) CheckHealthEvents();
    }
    
    public void GainHealth(float amount)
    {
        health += amount;
        if (health >= maxHealth) health = maxHealth; 
        if (_performHealthEvents) CheckHealthEvents();
    }

    public void TakeDamage(float amount)
    {
        LoseHealth(amount);
    }

    public void TakeDamage(IDamageDealer dealer)
    {
        LoseHealth(dealer.GetDamage());
    }
}