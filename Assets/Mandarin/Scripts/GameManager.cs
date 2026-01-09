 using UnityEngine;

public class GameManager :  MonoBehaviour
{
    public static GameManager Instance;     // 싱글톤


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

    [SerializeField] private int currentGold = 1000;
    //[SerializeField] private int rerollCost = 100;      // 리롤 비용

    // 객체가 생성될 때(Awake) 자기 자신을 Instance에 할당한다.
    private void Awake() => Instance = this;
    // 다른 스크립트에서 현재 골드가 얼마인지 읽기만할 때
    public int GetCurrentGold() => currentGold;


    //UI에서 카드를 사용하려고 할 때 호출되는 핵심 함수
    public bool TrySummonUnit(CardRawData data)
    {
        // 비용체크
       if(currentGold < data.cost)
       {
            Debug.Log("골드가 부족합니다!");
            return false;
        }

        // 골드 차감
        currentGold -= data.cost;

        SpawnUnit(data.cardName, data.type);
        //    // 실제 필드에서 유닛,타워,힐등 소환(Resources에서 로드 등)
        //    GameObject unit = Instantiate(Resources.Load<GameObject>(data.prefabPath));
        //    Debug.Log($"[Game] {data.cardName} 소환 완료! 남은 골드: {currentGold}");

        return true;
    }

    public void SpawnUnit(string _type, ResourceManager.DicType _layer)
    {
        switch (_layer)
        {
            case ResourceManager.DicType.Enemy:
                enemyMng.SpawnEnemy(_type);
                break;
            case ResourceManager.DicType.PlayerUnit:
                allyMng.SpawnAlly(_type);
                break;
            case ResourceManager.DicType.Tower:
                towerMng.BuildTower(_type);
                break;



        }

    }

    //랜덤버튼
    public void SpendGold(int amount)
    {
        // 입력받은 금액만큼 골드를 줄인다.
        currentGold -= amount;

        //여기에는 UI텍스트 (현재 골드표시)을 갱신하는 코드를 넣으면 좋다.
    }

}
