namespace KCH
{

    using UnityEngine;

    public class AllyUnitManager : MonoBehaviour
    {
        [SerializeField]
        private Transform spawnPoint = null;
        // private Queue<BaseAlly>          // 풀링용 리스트 
        private PlayerUnit curGo = null;

       

        public void SpawnAlly(string _type)
        {
            Debug.Log("SpawnAlly" + _type);
            curGo = ResourceManager.Instance.TowerPrefList[_type].GetComponent<PlayerUnit>();
        }


    }

}