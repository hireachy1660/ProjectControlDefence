using UnityEngine;
using System.Collections.Generic;   // List<T> 사용가능

public class CardManager : MonoBehaviour
{
    // 싱글톤을 안전하게 선언 - CardManager.Instance.함수() 로 접근 가능
    // private set - 외부에서 이 값을 변경 불가
    public static CardManager Instance { get; private set; }

    [Header("UI & Prefab")]
    public GameObject cardPrefab = null;    // 카드 UI 프리팹
    public Transform cardSpawnParent = null;    // 카드가 생성될 UI 부모( 하단 패널 )

    private const int MAX_SLOTS = 5;   // 항상 유지할 카드 개수

    private void Awake()
    {
        // 싱글톤 중복 방지 로직
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        //게임 시작 시 처음으로 5장을 채움
        FillEmptySlots();
    }

    // 핵심] 부족한 카드만큼 랜덤하게 채우기 
    public void FillEmptySlots()
    {
        // 부모 오브젝트가 연결되지 않았다면 실행하지 않음
        if(cardSpawnParent == null)
        {
            Debug.LogWarning("CardManager: cardSpawnParent가 인스펙터에서 할당되지 않았습니다.");
            return;
        }

        // 현재 떠있는 카드 개수 체크
        int currentChildCount = cardSpawnParent.childCount;
        // 부족한 개수 계산
        int needToFill = MAX_SLOTS - currentChildCount;

        if (needToFill <= 0) return;

        for(int i=0; i<needToFill; i++)
        {
            CreateRandomCard();
        }

    }

    // 리소스매니저에서 정보 받아서 카드 생성.
    private void CreateRandomCard()
    {
        // 리소스매니저가 있는지 체크
        if (ResourceManager.Instance == null) return;

        // 리소스 매니저에서 랜덤 데이터 하나 가져오기 (매니저 역할)
        // 리소스매니저 함수,클래스 수정하기.
        List<CardRawData> allData = ResourceManager.Instance.allCards;
        CardRawData randomData = allData[Random.Range(0, allData.Count)];

        // 카드UI생성(매니저 역할)
        GameObject newCard = Instantiate(cardPrefab, cardSpawnParent);

        // 지시 ( 카드가 알아서 하게 )
        if(newCard.TryGetComponent<Cards>(out Cards cardScript))
        {
            // 자, 이 데이터로 너의 몸을 꾸며라~
            cardScript.Setup(randomData);
        }
    }

    // 카드 사용(클릭) 시 호출
    public void OnCardSelected(CardRawData data, GameObject cardUI)
    {
        if (data == null) return;

        // 선택된 카드를 리스트/UI에서 제거
        Destroy(cardUI);

        // 카드가 사라졌으므로 다음카드 즉시 생성(0.2f후)
        Invoke("FillEmptySlots", 0.2f);

        // GameManager 에러가 난다면 GameManager 파일이 프로젝트에 있는지 확인하세요.
        // 유닛 소환 GameManager에게 요청
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartSummonProcess(data);
        }

        // 유닛 소환 GameManager에게 요청
        //TGameManager.Instance.StartSummonProcess(data);


    }

    

}