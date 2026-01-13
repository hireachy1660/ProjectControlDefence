using UnityEngine;
using System.Collections.Generic;

public class GameObjectList : MonoBehaviour
{
    private static GameObjectList instance;

    [SerializeField]
    private List<PlayerUnit> playerUnit;

    [SerializeField]
    private List<EnemyUnit> enemyUnit;

    [SerializeField]
    private List<BaseTower> towerUnit;
    private void Awake()
    {
        instance = this;
        playerUnit = new List<PlayerUnit>();
        enemyUnit = new List<EnemyUnit>();
        towerUnit = new List<BaseTower>();

    }

    private void Update()
    {
        EnemyUnit[] enemyList = FindObjectsByType<EnemyUnit>(FindObjectsSortMode.None);
        for(int i = 0; i<enemyList.Length; ++i)
        {
            if (!enemyUnit.Contains(enemyList[i]))
            {
                enemyUnit.Add(enemyList[i]);
            }
        }
        PlayerUnit[] playerList = FindObjectsByType<PlayerUnit>(FindObjectsSortMode.None);
        for (int i = 0; i < playerList.Length; ++i)
        {
            if (!playerUnit.Contains(playerList[i]))
            {
                playerUnit.Add(playerList[i]);
            }
        }

        BaseTower[] towerList = FindObjectsByType<BaseTower>(FindObjectsSortMode.None);
        for (int i = 0; i < towerList.Length; ++i)
        {
            if (!towerUnit.Contains(towerList[i]))
            {
                towerUnit.Add(towerList[i]);
                
            }
        }

    }
    public Transform[] playerUnitList()
    {
        Transform[] playerTr = new Transform[playerUnit.Count];
        for(int i= 0; i < playerUnit.Count; ++i)
        {
            playerTr[i] = playerUnit[i].transform;
        }

        return playerTr;
    }

    public Transform[] enemyUnitList()
    {
        Transform[] enemyTr = new Transform[enemyUnit.Count];
        for (int i = 0; i < enemyUnit.Count; ++i)
        {
            enemyTr[i] = enemyUnit[i].transform;
        }

        return enemyTr;
    }

    public Transform[] towerUnitList()
    {
        Transform[] towerTr = new Transform[towerUnit.Count];
        for (int i = 0; i < towerUnit.Count; ++i)
        {
            towerTr[i] = towerUnit[i].transform;
        }

        return towerTr;
    }
}

//using UnityEngine;

//public class GameObjectList : MonoBehaviour
//{
//    // [수정 1] 외부에서 접근 가능하도록 public 프로퍼티로 변경
//    public static GameObjectList Instance { get; private set; }

//    [SerializeField]
//    private Transform[] playerUnit;

//    [SerializeField]
//    private Transform[] enemyUnit;

//    private void Awake()
//    {
//        // 싱글톤 초기화
//        if (Instance != null && Instance != this)
//        {
//            Destroy(this.gameObject);
//            return;
//        }
//        Instance = this;

//        // ---------------------------------------------------------
//        // 적 유닛 찾기 및 배열 초기화
//        // ---------------------------------------------------------
//        EnemyUnit[] foundEnemies = FindObjectsByType<EnemyUnit>(FindObjectsSortMode.None);

//        // [수정 2] 찾은 개수만큼 배열의 방(메모리)을 먼저 만들어야 합니다!
//        enemyUnit = new Transform[foundEnemies.Length];

//        for (int i = 0; i < foundEnemies.Length; ++i)
//        {
//            enemyUnit[i] = foundEnemies[i].transform;
//        }

//        // ---------------------------------------------------------
//        // 플레이어 유닛 찾기 및 배열 초기화
//        // ---------------------------------------------------------
//        PlayerUnit[] foundPlayers = FindObjectsByType<PlayerUnit>(FindObjectsSortMode.None);

//        // [수정 2] 역시 배열 크기 할당 필수
//        playerUnit = new Transform[foundPlayers.Length];

//        for (int i = 0; i < foundPlayers.Length; ++i)
//        {
//            playerUnit[i] = foundPlayers[i].transform;
//        }
//    }

//    public Transform[] GetPlayerUnitList() // C# 네이밍 관습에 따라 대문자 시작 권장
//    {
//        return playerUnit;
//    }

//    public Transform[] GetEnemyUnitList()
//    {
//        return enemyUnit;
//    }

//    // 기존 호환성을 위해 소문자 메서드도 남겨둠 (필요 없으면 삭제하세요)
//    public Transform[] playerUnitList() => playerUnit;
//    public Transform[] EnemyUnitList() => enemyUnit;
//}
