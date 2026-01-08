namespace KCH
{
    using System.Collections;
    using UnityEngine;


    public class TowerManager : MonoBehaviour
    {
        //private Queue<BaseTower> towerPull;         // 풀링용 리스트 
        private BaseTower curGo = null;



        public void BuildTower(string _type)
        {
            Debug.Log("BuildTower" + _type);
            curGo = ResourceManager.Instance.TowerPrefList[_type].GetComponent<BaseTower>();
            //BaseTower inPullTower = null;

            //for (int i; i < towerPull.lenth; i++)
            //{
            //    if (towerPull[i].Type == curGo.Type)
            //    {
            //        inPullTower = towerPull[i];
            //        Instantiate(inPullTower);
            //        towerPull.RemoveAt(i);
            //        break;
            //    }
            //}
            Instantiate(curGo);


        }

        private IEnumerator FindBuildPos()
        {
            curGo.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            yield break;
        }

    }
 
}
