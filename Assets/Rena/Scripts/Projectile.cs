using UnityEngine;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float arcHeight = 2f;
    public bool isExplosive = false;
    public float explosionRadius = 2f;
    public bool isPiercing = false;

    private Collider target;
    private Vector3 startPos;
    private Vector3 targetLastPos;
    private float damage;
    private float progress = 0;
    private bool useArc = false;
    private Vector3 shootDirection;
    private List<GameObject> hitEnemies = new List<GameObject>();

    public void Setup(Collider _target, float _damage, bool _useArc)
    {
        target = _target;
        damage = _damage;
        useArc = _useArc;
        startPos = transform.position;
        progress = 0;

        if (target != null)
        {
            targetLastPos = new Vector3(target.bounds.center.x, transform.position.y, target.bounds.center.z);
            shootDirection = (targetLastPos - startPos).normalized;
        }
    }

    void Update()
    {
        // 물리 속도 강제 초기화 (추락 방지)
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 1. 관통형(대포) 로직
        if (isPiercing)
        {
            transform.position += shootDirection * speed * Time.deltaTime;
            transform.up = shootDirection;

            if (Vector3.Distance(startPos, transform.position) > 25f) Destroy(gameObject);
            return; // 대포는 여기서 끝 (아래 Lerp 로직 안 탐)
        }

        // 2. 일반/박격포 로직
        if (target != null) targetLastPos = target.bounds.center;

        float initialDist = Vector3.Distance(startPos, targetLastPos);
        if (initialDist < 0.1f) initialDist = 0.1f;

        progress += Time.deltaTime * speed / initialDist;
        Vector3 nextPos = Vector3.Lerp(startPos, targetLastPos, progress);

        if (useArc)
        {
            nextPos.y += Mathf.Sin(progress * Mathf.PI) * arcHeight;
        }
        else
        {
            if (targetLastPos - transform.position != Vector3.zero)
                transform.up = (targetLastPos - transform.position).normalized;
        }

        transform.position = nextPos;

        // 목표 지점 도달 시 처리
        if (progress >= 1f)
        {
            if (isExplosive)
            {
                Explode();
            }
            else
            {
                // [수정] 주석 해제하여 데미지 적용
                ApplySingleDamage();
            }

            Destroy(gameObject);
        }
    }

    void ApplySingleDamage()
    {
        // 타겟이 이미 죽었을 수도 있으므로 OverlapPoint 등으로 체크하는 것이 더 정확할 수 있습니다.
        if (target != null)
        {
            target.GetComponent<IDamageable>()?.TakeDamage(damage);
        }
        else
        {
            // 타겟이 없더라도 해당 위치의 적을 직접 찾아 데미지를 줍니다.
            Collider[] hit = Physics.OverlapSphere(transform.position, 0.1f);
            if (hit.Length > 0&& hit[0].CompareTag("Enemy"))
            {
                hit[0].GetComponent<IDamageable>()?.TakeDamage(damage);
            }
        }
    }

    void Explode()
    {
        
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                hit.GetComponent<IDamageable>()?.TakeDamage(damage);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tower")) return;

        if (other.CompareTag("Enemy"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable == null) return;

            if (isPiercing) // 대포
            {
                if (!hitEnemies.Contains(other.gameObject))
                {
                    damageable.TakeDamage(damage);
                    hitEnemies.Add(other.gameObject);
                }
            }
            else if (!isExplosive) // 아처 (날아가다 직접 부딪힌 경우)
            {
                damageable.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }

}