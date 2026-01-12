namespace KCH
{

    using UnityEngine;

    public class Nexus : MonoBehaviour , IDamageable
    {
        public delegate void NexusDestroydelegate(KCH.Nexus _nexus);

        private NexusDestroydelegate nexusDestroyCallback = null;

        public NexusDestroydelegate NexusDestroyCallback
        { set { nexusDestroyCallback = value; } }


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

        public void TakeDamage(float _dmg, IDamageable _target)
        {
            curHealth -= (int)_dmg;
            if (curHealth <= 0 )
            {
                nexusDestroyCallback?.Invoke(this);
                Destroy(this);
            }
        }

    }

}