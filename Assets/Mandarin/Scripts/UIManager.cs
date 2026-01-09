using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Sub Managers")]
    [SerializeField] private CardManager cardLogicManager;  // 연산 담당

    [Header("UI References")]
    [SerializeField] private GameObject cardPrefab = null;
    [SerializeField] private Transform cardContainer = null;
    [SerializeField] private GameObject shopPanel = null;

    private void Awake()
    {
        // shopPanel 끄기 ( 켜져있을경우 대비)
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }

    private void Start()
    {

        if (cardLogicManager == null)
        {
            Debug.LogError("UIManager에 CardLogicManager가 연결되지 않았습니다!");
            return;
        }
        ShowShopUI();
    }
    public void ShowShopUI()
    {
        if(shopPanel != null)
        {
            shopPanel.SetActive(true);  // shopPanel 활성화
        }
        InitHandUI();   // 랜덤카드 5장 생성
    }    
    public void InitHandUI()
    {
        // 기존 카드 싹 지우기 (가장 확실한 방법)
        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }

        // CardManager에게 처음 가질 카드 5개 데이터를 요청
        List<CardRawData> initialCards = cardLogicManager.GetInitialHand();

        if (initialCards.Count == 0)
        {
            Debug.LogWarning("가져온 카드 데이터가 없습니다.");
            return;
        }
        // 받은 데이터 개수만큼 루프를 돌며 실제 화면에 카드를 만든다.
        foreach (var data in initialCards)
        {
            CreateCardUI(data);
        }
        Debug.Log("UI 카드 생성 완료");

    }

    public void CreateCardUI(CardRawData data)
    {
        GameObject go = Instantiate(cardPrefab, cardContainer);
        UICard ui = go.GetComponent<UICard>();
        ui.Setup(data, this);
    }


    // 랜덤카드 클릭 시 흐름 제어
    public void OnCardClicked(CardRawData data, UICard cardScript)
    {
        // GameManager에게 이 카들 쓸 돈 있어? 있으면 소환 요청
        if (GameManager.Instance.TrySummonUnit(data))
        {
            float animTime = 0.25f;
            float cardWidth = 185f + 20f; // 너비 + 스패싱
            int clickedIndex = cardScript.transform.GetSiblingIndex();

            // 1. 새 카드 미리 생성 (맨 오른쪽에 생김)
            UICard nextCard = CreateCardUI_Return(cardLogicManager.GetRandomCardData());

            // 2. 새 카드는 화면 밖(오른쪽)에서 대기 (SetOffset 사용)
            nextCard.SetOffset(cardWidth);

            // 3. 클릭된 카드 뒤에 있는 모든 카드(방금 만든 새 카드 포함!)를 동시에 밀기
            for (int i = clickedIndex + 1; i < cardContainer.childCount; i++)
            {
                UICard otherCard = cardContainer.GetChild(i).GetComponent<UICard>();
                if (otherCard != null) StartCoroutine(otherCard.Co_MoveLeft(cardWidth, animTime));
            }

            // 4. 클릭된 카드는 제자리에서 사라짐
            StartCoroutine(cardScript.Co_FadeOutAndHide(animTime));

            // 5. 애니메이션이 끝난 후 죽은 카드 파괴 및 레이아웃 최종 정리
            StartCoroutine(Co_CleanupLayout(cardScript, animTime));

        }
        else
        {
            // 골드가 부족하다면 흔들리는 효과 같은 걸 줄 수 있다.
            Debug.Log("소환 실패: 골드 부족");
        }
    }
    // 랜덤버튼 클릭 시
    public void OnRerollButtonClicked()
    {
        // CardManager에게 리롤(골드차감)이 가능한지 물어봄
        if (cardLogicManager.TryProcessReroll())
        {
            // 성공했다면 기존 카드 UI 삭제
            foreach (Transform child in cardContainer)
            {
                Destroy(child.gameObject);
            }
            // 새 카드들로 다시 채우기
            Invoke("InitHandUI", 0.05f);
        }
        else
        {
            // 실패 시 알림( 흔들기연출 할꽈말꽈)
            Debug.Log("금액이 부족하여 리롤할 수 없습니다.");
        }

    }
   
    private IEnumerator Co_CleanupLayout(UICard deadCard, float delay)
    {
        yield return new WaitForSeconds(delay);

        // 여기서 비로소 죽은 카드를 지웁니다. 
        // 그러면 6개였던 카드가 5개가 되면서 레이아웃이 착 달라붙습니다.
        Destroy(deadCard.gameObject);

        yield return null; // 파괴된 프레임 대기
        LayoutRebuilder.ForceRebuildLayoutImmediate(cardContainer as RectTransform);
    }
    private UICard CreateCardUI_Return(CardRawData data)
    {
        GameObject go = Instantiate(cardPrefab, cardContainer);
        UICard ui = go.GetComponent<UICard>();
        ui.Setup(data, this);
        // 생성 직후 레이아웃을 갱신해야 정확한 SiblingIndex와 위치가 잡힙니다.
        LayoutRebuilder.ForceRebuildLayoutImmediate(cardContainer as RectTransform);
        return ui;
    }

}
