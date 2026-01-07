using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;   // List<T> 사용가능
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{

    [Header("Manager References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private UIManager UIManager;

    [Header("UI & Prefab")]
    [SerializeField] private GameObject cardPrefab = null;    // 카드 UI 프리팹
    [SerializeField] private Transform cardsContainer = null;    // 카드가 생성될 UI 부모( 하단 패널 )

    private const int MAX_SLOTS = 5;   // 항상 유지할 카드 개수
    private const float ANIM_SPEED = 0.25f;  // 애니메이션 속도

    private void Start()
    {
        //게임 시작 시 처음으로 5장을 채움
        FillEmptySlots();
    }

    // 핵심] 부족한 카드만큼 랜덤하게 채우기 
    public void FillEmptySlots()
    {
        // 부모 오브젝트가 연결되지 않았다면 실행하지 않음
        if (cardsContainer == null) return;

        // 현재 떠있는 카드 개수 체크
        int currentChildCount = cardsContainer.childCount;
        // 부족한 개수 계산
        int needToFill = MAX_SLOTS - currentChildCount;

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

        //////

        // 리소스 매니저에서 랜덤 데이터 하나 가져오기 (매니저 역할)
        // 리소스매니저 함수,클래스 수정하기.
        List<CardRawData> allData = ResourceManager.Instance.allCards;
        CardRawData randomData = allData[Random.Range(0, allData.Count)];

        // 카드UI생성(매니저 역할)
        GameObject newCard = Instantiate(cardPrefab, cardsContainer);

        ///

        //생성되자마자 투명하게 만들기(등장 전 튐 방지)
        if (newCard.TryGetComponent<CanvasGroup>(out CanvasGroup cg))
        {
            cg.alpha = 0;
            // 아주 짧은 시간 동안만 투명도를 올려주는 간단한 연출
            StartCoroutine(FadeInCard(cg));
        }

        // 지시 ( 카드가 알아서 하게 )
        if (newCard.TryGetComponent<Cards>(out Cards cardScript))
        {
            // 자, 이 데이터로 너의 몸을 꾸며라~
            cardScript.Setup(randomData, this);
        }
    }

    // 카드 사용(클릭) 시 호출
    public void OnCardSelected(CardRawData data, GameObject cardUI)
    {
        if (data == null) return;

        // 중복 클릭 방지
        cardUI.GetComponent<Button>().interactable = false;

        // 미리 새카드 맨 오른쪽에 생성
        CreateRandomCard();

        // 선택한 카드 너비 줄임
        StartCoroutine(SmoothRemoveFilm(cardUI));

        // 참조된 매니저 호출 -> 유닛 소환 GameManager에게 요청
        if(gameManager != null)
        {
            gameManager.StartSummonProcess(data);
        }
        if(UIManager != null)
        {
            UIManager.AddCardToHaondUI(data);
        }
    }

    // 랜덤버튼 눌렀을 때
    public void OnClickReroll()
    {
        // 골드 매니저가 있다면 골드 체크 
        // if( Gold < 2 ) return;

        // 기존 카드 삭제 및 재생성
        // 모든 자식을 돌면서 부뫄 관계를 끊고 해야함.
        foreach (Transform child in cardsContainer)
        {
            child.SetParent(null);
            Destroy(child.gameObject);
        }

        // 약간의 시간차 두고 다시 채우기 ( 삭제가 완료된 후 실행 )
        Invoke("FillEmptySlots", 0.05f);

        Debug.Log("상점 리롤 완료!");
    }

    private IEnumerator SmoothRemoveFilm(GameObject cardUI)
    {
        LayoutElement layout = cardUI.GetComponent<LayoutElement>();
        CanvasGroup canvasGroup = cardUI.GetComponent<CanvasGroup>();

        float startWidth = 210f;    // 현재 카드 너비
        float elapsed = 0f;

        while(elapsed < ANIM_SPEED)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / ANIM_SPEED;
            // Mathf.SmoothStep을 쓰면 훨씬 쫀득하게 된다.
            float currentWidth = Mathf.Lerp(startWidth, 0, Mathf.SmoothStep(0, 1, percent));

            if(layout != null)
            {
                layout.preferredWidth = currentWidth;
                layout.minWidth = currentWidth;
            }
            if(canvasGroup != null)
            {
                canvasGroup.alpha = 1 - percent;
            }
            yield return null;
        }

        // 완전히 사라지며 정리 (잔상 방지)
        cardUI.SetActive(false);
        Destroy(cardUI);
    }
    private IEnumerator FadeInCard(CanvasGroup cg)
    {
        float elapsed = 0;
        while (elapsed < 0.2f)   // 아주 빠르게 등장
        {
            elapsed += Time.deltaTime;
            cg.alpha = elapsed / 0.1f;
            yield return null;
        }
        cg.alpha = 1;
    }


}