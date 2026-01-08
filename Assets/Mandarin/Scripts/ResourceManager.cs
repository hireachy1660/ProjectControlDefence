using System.Collections.Generic;
using UnityEngine;
using static BaseTower;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    // 각 유닛 프리팹을 보관 
    // 아래의 유닛 및 타워 리소스의 자료형은 각 베이스 스크립트 형으로 변경할 것
    [SerializeField]
    private Dictionary<string, GameObject> towerPrefList = new Dictionary<string, GameObject>();
    [SerializeField]
    private Dictionary<string, GameObject> enemyPrefList = new Dictionary<string, GameObject>();
    [SerializeField]
    private Dictionary<string, GameObject> allyPrefList = new Dictionary<string, GameObject>();

    public Dictionary<string, GameObject> TowerPrefList
    { get { return towerPrefList; } }
    public Dictionary<string, GameObject> EnemyPrefList
    { get { return enemyPrefList; } }
    public Dictionary<string, GameObject> AllyPrefList
    { get { return allyPrefList; } }

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

        GameObject[] gos = Resources.LoadAll<GameObject>("Prefabs");
        foreach (GameObject go in gos)
        {
            switch (go.tag)
            {
                case "Ally":
                    //allyPrefList.Add(go.GetComponent<BaseAlly>);
                    Debug.Log("allyPrefList.Add(go.GetComponent<BaseAlly>)");
                    break;
                case "Enemy":
                    //allyPrefList.Add(go.GetComponent<BaseEnemy>);
                    Debug.Log("allyPrefList.Add(go.GetComponent<BaseEnemy>)");
                    break;
                case "Tower":
                    towerPrefList.Add(go.GetComponent<BaseTower>().Type.ToString(),go);
                    Debug.Log("allyPrefList.Add(go.GetComponent<BaseTower>)");
                    break;
                case null:
                    Debug.Log("ResouceManager Loaded Somthing Worng");
                    break;
            }
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

    // 리소스 메니저에 유닛들을 분류하여 저장하는 방법
    // 유닛 프리팹은 자신의 부모 클래스 아래에 있는 자식 개수 만큼의 이넘을 가진다 자신의 클래스에 맞는 이넘값을 저장한 매개 변수를 가지며 이에 맞는 태그를 설정한다
    // 로드로 찾은 게임 오브젝트의 태그를 검사하여 태그에 맞는 딕셔너리에 자신을 게임 오브젝트로 저장
    // 또한 카드 로우 데이터는 이 이넘 값을 자신의 이넘 값으로 지정, 그러면 딕셔너리의 키 값은 해당 자신에 맞는 이넘 값으로 지정 후 자신의 베이스 스크립트형으로 벨류를 지정 
    // 이후 버튼이 클릭되면 카드 메니저를 호출할때 자신의 이넘 값및 태그를 전송
    // 이걸 카드메니저가 게임메니저에 전달 한 뒤 게임 메니저는 태그로 검사하여 각 메니저에 키값인 이넘 값을 전달
    // 해당 매니저는 그 키 값을 자신의 담당 리소스 메니저 딕셔너리에 값을 넣어 게임 오브젝트를 받아옴
    // 해당 매니저는 인스턴티에이트전 해당 이넘 값으로 자신의 풀안에 있는 베이스 클래스의 이넘값을 비교하여 있는면 초기화 없으면 생성후 큐에 추가
    // 소지금 값이 변하면 단순 현재 소지금을 인트로 각 카드 객체에 던짐 그러면 값을 비교하여 자신을 활성화 
    
}