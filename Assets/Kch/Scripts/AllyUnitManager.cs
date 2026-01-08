namespace KCH
{

    using UnityEngine;

    public class AllyUnitManager : MonoBehaviour
    {
        // private Queue<BaseAlly>          // 풀링용 리스트 

        public void SpawnAlly(string _type)
        {
            Debug.Log("SpawnAlly" + _type);
        }


    }

}