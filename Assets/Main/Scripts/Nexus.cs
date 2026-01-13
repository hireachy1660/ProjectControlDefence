namespace KCH
{

    using UnityEngine;

    public class Nexus : BaseTower , IDamageable
    {
        public delegate void NexusDestroydelegate(KCH.Nexus _nexus);

        private NexusDestroydelegate nexusDestroyCallback = null;

        public NexusDestroydelegate NexusDestroyCallback
        { set { nexusDestroyCallback = value; } }


        //private float maxHealth = 100f;
        //private float curHealth = 100f;



        private void OnEnable()
        {
            curHealth = MaxHealth;
        }

        public override void TakeDamage(float _dmg, IDamageable _target)
        {
            curHealth -= (int)_dmg;
            Debug.Log("curHealth :" +  curHealth);
            if (curHealth <= 0 )
            {
                nexusDestroyCallback?.Invoke(this);
                Destroy(this);
            }
        }

        protected override void Attack()
        {
            Debug.Log("Base not have Attack");
        }

    }

}