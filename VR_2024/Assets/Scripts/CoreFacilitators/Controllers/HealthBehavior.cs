using UnityEngine;
using UnityEngine.Events;

public class HealthBehavior : MonoBehaviour, IDamagable
{
    public UnityEvent onHealthGained, onHealthLost, onHealthDepleted;
    
    [SerializeField] [ReadOnly] private float currentHealth;
    public float maxHealth;

    public float health
    {
        get => currentHealth;
        set => currentHealth = value;
    }
    public void Start()
    {
        if (maxHealth <= 0) maxHealth = 1;
        health = maxHealth;
    }
    
    public void SetHealth(float newHealth)
    {
        health = newHealth;
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
    }
    
    public void GainHealth(float amount)
    {
        health += amount;
        if (health >= maxHealth) health = maxHealth; 
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