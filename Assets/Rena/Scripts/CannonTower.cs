using UnityEngine;

public class CannonTower : BaseTower
{
    protected override void Attack()
    {
        if (currentTarget == null) return;

        GameObject projGO = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projectile = projGO.GetComponent<Projectile>();
       
        if (projectile != null)
        {
            projectile.isPiercing = true; //관통공격 활성화
            //대포는 보통 직선으로 빠르게 날아가는것이 좋으므로 useArc는 false
            projectile.Setup(currentTarget, damage, false);

            //대포알이 화면 밖으로 영원히 날아가지 않도록 일정시간 후 파괴설정
            Destroy(projGO, 4f);
        }
    }
}