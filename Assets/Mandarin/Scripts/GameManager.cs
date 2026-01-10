 using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class GameManager :  MonoBehaviour
{
    public static GameManager Instance;     // 싱글톤

    public delegate void SpawnUnitdelegate(IDamageable _target);

    private SpawnUnitdelegate spawnUnitCallback;

    public SpawnUnitdelegate SpawnUnitCallback
    { set { spawnUnitCallback = value; } get { return spawnUnitCallback; } }

    #region 메니저 멤버 변수
    [Header("Managers")] 
    [SerializeField]
    private List<KCH.Nexus> nexuses = new List<KCH.Nexus>();
    [SerializeField]
    private KCH.AllyUnitManager allyMng = null;
    [SerializeField]
    private KCH.EnemyUnitManager enemyMng = null;
    [SerializeField]
    private KCH.TowerManager towerMng = null;
    [SerializeField]
    private UIManager uiMng = null;
    #endregion

    [SerializeField] private int currentGold = 1000;
    //[SerializeField] private int rerollCost = 100;      // 리롤 비용
    [SerializeField]
    private int secPerGold = 1;

    // 객체가 생성될 때(Awake) 자기 자신을 Instance에 할당한다.
    private void Awake() => Instance = this;
    // 다른 스크립트에서 현재 골드가 얼마인지 읽기만할 때
    public int GetCurrentGold() => currentGold;

    #region 라이프 사이클
    private void Start()
    {
        SetNexuses();
        uiMng.UpdateGold(currentGold);
        StartCoroutine(GetGoldCoroutine(secPerGold));
        SpawnUnitCallback = uiMng.SetHPBar;
    }
    #endregion


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
        uiMng.UpdateGold(currentGold);

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
        currentGold -= amount;
        uiMng.UpdateGold(currentGold);
    }

    private IEnumerator GetGoldCoroutine(int _goldPerSecond)
    {
        while(true)
        {
            currentGold += _goldPerSecond;
            uiMng.UpdateGold(currentGold);
            yield return new WaitForSeconds(1f);
        }
    }

    private void SetNexuses()
    {
        foreach(KCH.Nexus data in nexuses)
        {
            data.NexusDestroyCallback = NexusDestroy;
        }
    }

    private void NexusDestroy(KCH.Nexus _nexus)
    {
        nexuses.Remove( _nexus );

        if ( nexuses.Count == 0 )
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Is Over");
        StopCoroutine(GetGoldCoroutine(secPerGold));
    }

}
