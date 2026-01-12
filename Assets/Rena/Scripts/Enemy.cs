using UnityEngine;

// IDamageable 인터페이스를 상속받아야 투사체가 인식합니다.
public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField]
    private float maxHealth = 100f;
    private float curHealth = 100f;

    public float MaxHealth
    { get { return maxHealth; } }
    public float CurHealth
    { get { return curHealth; } }

    private void OnEnable()
    {
        curHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        curHealth -= damage;
        Debug.Log($"적 체력 감소: {curHealth}"); // 콘솔창에서 확인용

        if (curHealth <= 0)
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