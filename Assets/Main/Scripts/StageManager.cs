using KCH;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct EnemySpawnData
{
    public EnemyUnitManager.EEnemyType EnemyType;      // 리소스 매니저에서 찾을 키값 (또는 이넘)
    public int count;             // 소환할 마릿수
    [Range(1f, 100f)]
    public float spawnInterval;   // 마리당 소환 간격
}

// 2단계: 하나의 웨이브 구성 (여러 종류의 적을 담음)
[Serializable]
public struct WaveData
{
    public string waveName;                // 웨이브 이름 (예: "고블린 습격")
    public List<EnemySpawnData> enemyList; // 이 웨이브에 등장할 적 목록
    public float waveDelay;                // 이 웨이브가 시작되기 전 대기 시간
}

public class StageManager : MonoBehaviour
{
    public delegate void SetWaveTimerdelegate(float _time);
    public delegate void SetWaveEnemyCountdelegate(int _count, int _curWaveNum);

    private SetWaveTimerdelegate setWaveTimerCallback;
    private SetWaveEnemyCountdelegate setWaveEnemyCountCallback;

    public SetWaveTimerdelegate SetWaveTimerCallback
    { set { setWaveTimerCallback = value; } }
    public SetWaveEnemyCountdelegate SetWaveEnemyCountCallback
    { set { setWaveEnemyCountCallback = value; }}

    [SerializeField]
    private List<WaveData> waveList;

    [SerializeField]
    private int curWaveEnemyCount = 0;

    private int maxWaveNum = 0;

    private void Start()
    {
        StartCoroutine(StartStageCoroutine());
        maxWaveNum = waveList.Count;
    }

    private IEnumerator StartStageCoroutine()
    {
        foreach (WaveData data in waveList)
        {
            curWaveEnemyCount = 0;
            foreach (EnemySpawnData enemySpawnData in data.enemyList)
            {
                curWaveEnemyCount += enemySpawnData.count;
            }
            setWaveTimerCallback?.Invoke(data.waveDelay);
            setWaveEnemyCountCallback?.Invoke(curWaveEnemyCount, maxWaveNum);

            yield return new WaitForSeconds(data.waveDelay);
            for (int i = 0; i < data.enemyList.Count; i++)
            {

             for(int j = 0; j < data.enemyList[i].count; j++)
                {
                    GameManager.Instance.SpawnUnit(data.enemyList[i].EnemyType.ToString(), ResourceManager.DicType.Enemy);
                    //yield return new WaitForSeconds(data.enemyList[i].spawnInterval);
                    yield return new WaitForSeconds(2f);
                }
            }

            yield return new WaitUntil(() => curWaveEnemyCount <= 0);
        }

        Debug.Log("All Wave Done");
        yield break;
    }

    public void UpdateRemainingEnemy()
    {
        curWaveEnemyCount--;
    }

}
