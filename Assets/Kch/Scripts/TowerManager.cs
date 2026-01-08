namespace KCH
{
    using System.Collections;
    using UnityEngine;


    public class TowerManager : MonoBehaviour
    {
        // private Queue<BaseTower> towerPull         // 풀링용 리스트 
        private GameObject curGo = null;



        public void BuildTower(int _objNum)
        {
            Debug.Log("BuildTower" + _objNum);
            //curGo = ResourcesManager.Instance.TowerPrefList[_objNum];
            //BaseTower inPullTower = null;

            //for (int i; i < towerPull.lenth; i++)
            //{
            //    if (towerPull[i].type == curGo.type)
            //        inPullTower = towerPull[i];
            //    else return;
            //}
            Instantiate(curGo);
            
            
        }

        //private IEnumerator FindBuildPos()
        //{
        //    curGo.transform.position = Camera.main.ScreenToWorldPoint()
        //}
        
    }
 
}
