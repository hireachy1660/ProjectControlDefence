namespace KCH
{

    using UnityEngine;

    public class Nexus : MonoBehaviour
    {
        public delegate void NexusDestroydelegate();

        private NexusDestroydelegate nexusDestroyCallback = null;

        public NexusDestroydelegate NexusDestroyCallback
        { set { nexusDestroyCallback = value; } }


        [SerializeField]
        private int hp = 10;

        public void OnDamage(int _dmg)
        {
            hp -= _dmg;
            if (hp <= 0 )
            {
                nexusDestroyCallback?.Invoke();
            }
        }
    }

}