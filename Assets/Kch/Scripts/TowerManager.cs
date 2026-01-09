namespace KCH
{
    using System.Collections;
    using UnityEditor.UIElements;
    using UnityEngine;


    public class TowerManager : MonoBehaviour
    {
        //private Queue<BaseTower> towerPull;         // 풀링용 리스트 
        private BaseTower curGo = null;
        private BaseTower curBuildingTower = null;

        [SerializeField]    // 초기에 대상 레이어(바닥)를 선택 필수
        private LayerMask flootlayer;



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

            curBuildingTower = Instantiate(curGo);
            StartCoroutine(BuildMode());


        }

        private IEnumerator BuildMode()
        {
            curBuildingTower.enabled = false;

            while (curBuildingTower != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 50f, flootlayer))
                {
                    curBuildingTower.transform.position = hit.point + (Vector3.up *1f);
                }
                if (Input.GetMouseButtonDown(0))
                {
                    break;
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    curBuildingTower.gameObject.SetActive(false);
                    break;
                }
                    yield return null;
            }
            curBuildingTower = null;
            yield break;
        }

    }
 
}
