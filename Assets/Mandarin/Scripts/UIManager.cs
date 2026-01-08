using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("Sub Managers")]
    [SerializeField] private CardManager cardLogicManager;  // 연산 담당

    [Header("UI References")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardContainer;

    private const float ANIM_SPEED = 0.25f;     // 애니메이션 속도

    private void Start()
    {
        if (cardLogicManager == null)
        {
            Debug.LogError("UIManager에 CardLogicManager가 연결되지 않았습니다!");
            return;
        }
        InitHandUI();
    }

    public void InitHandUI()
    {
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
        if (data == null) return;
        GameObject newCardObj = Instantiate(cardPrefab, cardContainer);
        UICard cardScript = newCardObj.GetComponent<UICard>();
        cardScript.Setup(data, this);
    }

    // 랜덤카드 클릭 시 흐름 제어
    public void OnCardClicked(CardRawData data, UICard cardScript)
    {
        // GameManager에게 이 카들 쓸 돈 있어? 있으면 소환 요청
        if (GameManager.Instance.TrySummonUnit(data))
        {
            // 허락이 떨어졌을 때만 카드 교체 및 애니메이션 발생
            // 새 카드 미리 생성(오른쪽 끝에 붙음)
            CardRawData newData = cardLogicManager.GetRandomCardData();
            CreateCardUI(newData);

            // 현재 클릭된 카드의 축소 애니메이션 시작
            StartCoroutine(cardScript.Co_ShrinkAndDestroy(ANIM_SPEED));
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
}
