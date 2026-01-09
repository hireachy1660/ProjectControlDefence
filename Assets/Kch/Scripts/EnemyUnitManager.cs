namespace KCH
{
    using System.Collections.Generic;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;

    public class EnemyUnitManager : MonoBehaviour
    {
        private EnemyUnit curEnemyScript = null;
        private GameObject curGo = null;

        [SerializeField]
        private Transform[] SpawnTrs = null;
        [SerializeField] // 풀링 딕셔너리
        private Dictionary<EEnemyType, Queue<EnemyUnit>> EnemypullingDic = new Dictionary<EEnemyType, Queue<EnemyUnit>>();

        public enum EEnemyType
        {
            Enemy
        }

        private int curSpawnPos = 0;

        private int CurSpawnPos
        { 
            set { curSpawnPos = value % SpawnTrs.Length; } 
            get { return curSpawnPos; } 
        }


        public void SpawnEnemy(string _type)
        {
            GameObject go = null;
            if(ResourceManager.Instance.EnemyPrefList.TryGetValue(_type,out go))
            {
                Instantiate(go);    // 풀에 존재하는지 검사하는 로직 및 풀에서 제거하는 로직 필요
                curGo.transform.position = SpawnTrs[CurSpawnPos].transform.position;

                curEnemyScript = curGo.GetComponent<EnemyUnit>();
                // 자식의 게으른 초기화 또는 프로퍼티를 호출하며 사망 델리게이트에 넣을 메소드를 넣는다

                CurSpawnPos++;
            }
            else
            { Debug.Log("EnemyUnitManager.SpawnEnemy.Cant Find In ResourceManager"); }
        }

        private void OnDeadEnemy(EnemyUnit _unit)// 타입도 넣어 줄것
        {
            _unit.gameObject.SetActive(false);
            //TryDead(EnemyUnit _enemyUnit);
        }

        //private void TryDead(EnemyUnit _enemyUnit)
        //{
        //        Queue<EnemyUnit> nowPull;
        //        if (EnemypullingDic.TryGetValue(_enemyUnit.type, out nowPull))
        //        {
        //            nowPull.Enqueue(_enemyUnit);
        //            _enemyUnit.gameObject.SetActive(false);
        //        }
        //        else
        //        {
        //            EnemypullingDic.Add(_enemyUnit.type, new Queue<EnemyUnit>());
        //            EnemypullingDic[_enemyUnit.type].Enqueue(_enemyUnit);
        //        }
        //}

        private void TrySpawn(EnemyUnit _unit)
        {
        //        EnemyUnit pulledUnit;
        //        if (EnemypullingDic[_enemyUnit.type].TryDequeue(out pulledUnit))
        //        {
        //            pulledUnit.gameObject.SetActive(true);
        //            pulledUnit.transform.position = SpawnTrs[CurSpawnPos].position;
        //        }
        //        else
        //        {
        //            SpawnEnemy(_enemyUnit.type.ToString());
        //        }

        }
    }
}


