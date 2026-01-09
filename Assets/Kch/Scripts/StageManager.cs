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

    [SerializeField]
    private List<EnemySpawnData> waveList;
    [SerializeField]
    private float ReadyTime = 20f;

    private void Start()
    {
        
    }

    private IEnumerator StartStageCoroutine()
    {
        yield return new WaitForSeconds(ReadyTime);


        yield break;
    }


}
