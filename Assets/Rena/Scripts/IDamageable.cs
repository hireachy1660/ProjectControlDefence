public interface IDamageable
{
    float CurHealth { get; }
    float MaxHealth { get; }

    void TakeDamage(float damage);
}