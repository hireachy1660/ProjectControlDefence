namespace KCH
{
    using System.Collections;
    using System.Data.SqlTypes;
    using UnityEngine;

    public class DefenseGameManager : MonoBehaviour
    {
        [SerializeField]
        private KCH.Nexus[] nexuss = new KCH.Nexus[0];
        [SerializeField]
        private KCH.AllyUnitManager allyMng = null;
        [SerializeField]
        private KCH.EnemyUnitManager enemyMng = null;
        [SerializeField]
        private KCH.TowerManager towerMng = null;
        [SerializeField]
        private CardManager cardMng = null;

        private int money = 0;

        private void Start()
        {
            StartCoroutine(GetMoney());
        }

        private IEnumerator GetMoney()
        {
            while (true)
            {
                SetMoney(1);
                yield return new WaitForSeconds(1f);
            }
        }

        // 소지금이 변화 할 때마다 호출 되는 메소드 유아이에 버튼 활성화 여부를 위해 현재 금액을 카드 매니저 및 유아이 매니저에게 넘겨 주는 곳
        private void SetMoney(int _money)
        {
            money += _money;
            // 유아이 매니저 및 카드 매니저의 메소드를 호출
        }

        // 유닛을 스폰하기 위해서는 리소스 메니저의 배열 내부의 번호와 해당 프리팹의 태그를 델리게이트 매개변수에 담아 사용)
        private void SpawnUnit(string _type, string _tag)
        {
            switch(_tag)
            {
                case "Enemy":
                    enemyMng.SpawnEnemy(_type);
                    break;
                case "Ally":
                    allyMng.SpawnAlly(_type);
                    break;
                case "Tower":
                    towerMng.BuildTower(_type);
                    break;



            }
                
        }


    // 게임 메니저 

    // 역할
    // 카드 메니저로 부터 소환 된 카드 정보 받기 
    // 해당 정보를 토대로 유효한 매니저에 정보를 전달

    // 이때 카드로 소환 되는 아군 유닛과 타워는 카드 매니저에서 받고 
    // 적 유닛은 스테이지 메니저로 부터 받음 

    // 재화는 게임 메니저가 가짐
    // 자식 객체로 거점 오브젝트를 가짐, 체력은 거점 오브젝트가 가짐

    // 따라서 게임 오버에 대한 검사는 거점 파괴 콜백을 통해 검사하고 이때 파괴 되지 않은 거점이 없으면 게임 오버
    // 그러면 승리 조건? 스테이지 매니저가 마지막 스테이지에 콜백을 보내고 적 유닛 매니저가 콜백을 받은 이후 시점에 자신이 관리하는 적 유닛이 모두 0이 되면 다시 콜백 해서 게임매니저가 받아서 승리

    // 


        
    }

}