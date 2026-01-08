namespace KCH
{
    using System.Collections.Generic;
    using UnityEngine;

    public class ResourcesManager : MonoBehaviour
    {
        public static ResourcesManager Instance { get; private set; }

        // 아래의 유닛 및 타워 리소스의 자료형은 각 베이스 스크립트 형으로 변경할 것
        [SerializeField]
        private List<GameObject> towerPrefList = new List<GameObject>();
        [SerializeField]
        private List<GameObject> enemyPrefList = new List<GameObject>();
        [SerializeField]
        private List<GameObject> allyPrefList = new List<GameObject>();

        public List<GameObject> TowerPrefList
        { get { return towerPrefList; } }
        public List<GameObject> EnemyPrefList
        { get { return enemyPrefList; } }
        public List<GameObject> AllyPrefList
        { get { return allyPrefList; } }


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            GameObject[] gos = Resources.LoadAll<GameObject>("Prefabs");
            foreach (GameObject go in gos)
            {
                switch(go.tag)
                {
                    case "Ally":
                        //allyPrefList.Add(go.GetComponent<BaseAlly>);
                        Debug.Log("allyPrefList.Add(go.GetComponent<BaseAlly>)");
                        break;
                    case "Enemy":
                        //allyPrefList.Add(go.GetComponent<BaseEnemy>);
                        Debug.Log("allyPrefList.Add(go.GetComponent<BaseEnemy>)");
                        break;
                    case "Tower":
                        //allyPrefList.Add(go.GetComponent<BaseTower>);
                        Debug.Log("allyPrefList.Add(go.GetComponent<BaseTower>)");
                        break;
                    case null:
                        Debug.Log("ResouceManager Loaded Somthing Worng");
                        break;
                }
            }
        }

    }


}
