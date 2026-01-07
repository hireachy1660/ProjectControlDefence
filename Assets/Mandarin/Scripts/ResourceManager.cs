using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    // 자료구조] 이름을 키(key)로 해서 프리팹을 빠르게 찾기 위한 사전형태
    private Dictionary<string, GameObject> prefabDic = new Dictionary<string, GameObject>();

    // 테스트를 위해 인스펙터에서 카드 3~5장 정도 직접 리스트 넣기
    public List<CardRawData> allCards = new List<CardRawData>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 시작하자마자 리소스를 로드하여 자료구조화함
        LoadAllPrefabs();
    }
    private void LoadAllPrefabs()
    {
        //리소시스/프리팹스 폴더안의 모든 게임오브젝트를 가져온다.
        GameObject[] objs = Resources.LoadAll<GameObject>("Prefabs");

        foreach (GameObject obj in objs)
        {
            // 프리팹 이름을 키값으로 딕셔너리에 저장( 나중에 호출)
            if (!prefabDic.ContainsKey(obj.name))
            {
                prefabDic.Add(obj.name, obj);
            }

            // 테스트용 프리팹 정보로 카드 데이터 생성
            // 실제 프로젝트에서는 엑셀이나 JSON데이터를 섞어서 사용
            // 지금은 프리팹 이름과 정보를 매칭해서 리스트 넣는다.
            CardRawData newData = new CardRawData();
            newData.cardName = obj.name;
            newData.prefabPath="Prefabs/" + obj.name;   // 경로저장

            // 임시 코스트 설정(이름에따라 설정하는 로직을 넣을수있다)
            newData.cost = Random.Range(1, 6);
                
            allCards.Add(newData);

        }
        Debug.Log($"[ResourceManager] 총 {allCards.Count}개의 리소스를 로드했습니다.");

    }

    // 이름만 주면 프리팹을 바로 반환 함수
    public GameObject GetPrefab(string prefabName)
    {
        if (prefabDic.TryGetValue(prefabName, out GameObject go))
        {
            return go;
        }
        Debug.LogError($"{prefabName} 프리팹을 찾을 수 없습니다.");
        return null;
    }

}