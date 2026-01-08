using UnityEngine;

// IDamageable 인터페이스를 상속받아야 투사체가 인식합니다.
public class Enemy : MonoBehaviour, IDamageable
{
    public float health = 100f;

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log($"적 체력 감소: {health}"); // 콘솔창에서 확인용

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // 죽는 효과나 사운드를 넣기 위해 함수로 분리하는 것이 좋습니다.
        Destroy(gameObject);
    }
}