using UnityEngine;

public class GameObjectList : MonoBehaviour
{
    private static GameObjectList instance;

    [SerializeField]
    private Transform[] playerUnit;

    [SerializeField]
    private Transform[] enemyUnit;
    private void Awake()
    {
        instance = this;
    }

    public Transform[] playerUnitList()
    {
        return playerUnit;
    }

    public Transform[] EnemyUnitList()
    {
        return enemyUnit;
    }
}
