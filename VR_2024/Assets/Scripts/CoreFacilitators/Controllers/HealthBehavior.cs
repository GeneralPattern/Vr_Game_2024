using UnityEngine;
using UnityEngine.Events;

public class HealthBehavior : MonoBehaviour, IDamagable
{
    public UnityEvent onHealthGained, onHealthLost, onThreeQuarterHealth, onHalfHealth, onQuarterHealth, onHealthDepleted;
    
    [SerializeField] [ReadOnly] private float currentHealth;
    private float _previousCheckHealth;
    public float maxHealth;

    public float health
    {
        get => currentHealth;
        set => currentHealth = value;
    }

    private void Start()
    {
        if (maxHealth <= 0) maxHealth = 1;
        health = maxHealth;
    }
    
    private void CheckHealthEvents()
    {
        if (health == maxHealth || health == 0) return;
        if (health <= maxHealth * 0.75f && health >= maxHealth * 0.5f) onThreeQuarterHealth.Invoke();
        else if (health <= maxHealth * 0.5f && health >= maxHealth * 0.25f) onHalfHealth.Invoke();
        else if (health <= maxHealth * 0.25f && health >= maxHealth * 0) onQuarterHealth.Invoke();
    }
    
    public void SetHealth(float newHealth)
    {
        health = newHealth;
        if (health > maxHealth) health = maxHealth;
        if (health < 0) health = 0;
        CheckHealthEvents();
    }
    
    public void SetMaxHealth(float newMax)
    {
        maxHealth = newMax;
    }

    public void LoseHealth(float amount)
    {
        health -= amount;
        if (health <= 0) onHealthDepleted.Invoke();
        else onHealthLost.Invoke();
        CheckHealthEvents();
    }
    
    public void GainHealth(float amount)
    {
        health += amount;
        if (health >= maxHealth) health = maxHealth; 
        CheckHealthEvents();
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