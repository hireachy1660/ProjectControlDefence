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
        private int hp = 10;

        public void TakeDamage(float _dmg)
        {
            hp -= (int)_dmg;
            if (hp <= 0 )
            {
                nexusDestroyCallback?.Invoke(this);
                Destroy(this);
            }
        }

    }

}