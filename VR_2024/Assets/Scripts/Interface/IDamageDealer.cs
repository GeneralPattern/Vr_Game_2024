public interface IDamageDealer
{
    void DealDamage(IDamagable target, float amount);
    void DealDamage(IDamagable target);
    float GetDamage();
}
