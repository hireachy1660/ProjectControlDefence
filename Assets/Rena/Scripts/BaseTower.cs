using UnityEngine;

public abstract class BaseTower : MonoBehaviour, IDamageable
{
    [Header("Base Settings")]
    public float health = 100f;
    public float range = 5f;
    public float attackRate = 10f;
    public float damage = 10f;
    public LayerMask targetLayer;

    [Header("Raferences")]
    public GameObject projectilePrefab;  //투사체 프리맵
    public Transform firePoint;          //발사 위치

    protected float nextAttackTime;
    protected Transform currentTarget;

    //타워 매니저가 풀링에서 꺼낸후 초기화할때 사용
    public virtual void Initializ()
    {
        health = 100f;  // 기본 체력 초기화
        nextAttackTime = 0f;
    }

    protected virtual void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            FindTarget();
            if (currentTarget != null)
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    //적을 찾는 로직
    protected virtual void FindTarget()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, range, targetLayer);
        if (enemies.Length > 0)
        {
            //가장 가까운 적을 타켓으로 설정
            currentTarget = enemies[0].transform;
        }
        else
        {
            currentTarget = null;
        }
    }

    //실제 공격 방식은 자식클래스에서 재정의
    protected abstract void Attack();

    public virtual void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0) Die();
    }

    protected virtual void Die()
    {
        //타워 매니저의 풀로 돌아가거나 파괴됨
        gameObject.SetActive(false);
    }

    //에디터 사거리 시각화
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow
            ;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}