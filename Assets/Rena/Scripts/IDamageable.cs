using UnityEngine;

public interface IDamageable
{
    float CurHealth { get; }
    float MaxHealth { get; }
    Transform transform { get; }
    void TakeDamage(float damage);
}