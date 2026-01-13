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
        enemyUnit.RemoveAll(x => x == null);
        playerUnit.RemoveAll(x => x == null);
        towerUnit.RemoveAll(x => x == null);
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

