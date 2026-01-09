namespace KCH
{

    using UnityEngine;

    public class EnemyUnitManager : MonoBehaviour
    {
        // private Queue<BaseEnemy>          // 풀링용 리스트 

        public void SpawnEnemy(string _type)
        {
            Debug.Log("SpawnEnemy" + _type);
        }
    }

}
