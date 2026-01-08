using UnityEngine;

public class MortarTower : BaseTower

{
    public float explosionRadius = 3f;

    
    protected override void Attack()
    {
        if (currentTarget == null) return;
        
        GameObject projGO = Instantiate (projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projectile = projGO.GetComponent<Projectile>();

        if (projectile != null)
        {
            //박격포 특유의 투사체 설정 (폭발 속성 활성화)
            projectile.isExplosive = true;
            projectile.explosionRadius = explosionRadius;
            projectile.Setup(currentTarget, damage, true);  //ture은 포물선 사용여부

        }

    }
}
