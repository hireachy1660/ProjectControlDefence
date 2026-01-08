using UnityEngine;

public class ArcherTower : BaseTower
{

    protected override void Attack()
    {
        if (currentTarget == null) return;

        GameObject projGO = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projectile = projGO.GetComponent<Projectile>();

        if (projectile != null)
        {
            //아처 특유의 투사체 설정 ( 직선 혹은 낯은 곡선)
            projectile.Setup(currentTarget, damage, false);
        }
    }
}   