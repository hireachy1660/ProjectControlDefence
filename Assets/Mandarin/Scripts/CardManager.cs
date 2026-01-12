using UnityEngine;
using System.Collections.Generic;

// 업로드 후
// 카드 연산 및 데이터 관리 - 순수하게 어떤 카드가 뽑혔는지, (리롤 비용은 얼마인지)
public class CardManager : MonoBehaviour
{
    //public delegate void CardSelecteddelegate(string _name, string _tag);
    //private CardSelecteddelegate cardSelectedCallback;
    //public CardSelecteddelegate CardSelectedCallback
    //{ set { cardSelectedCallback = value; } }




    // 현재 뽑힌 카드들의 데이터 정보를 저장하는 리스트.
    public List<CardRawData> currentHandData = new List<CardRawData>();
    [SerializeField]
    private const int MAX_SLOTS = 5;        // 항상 유지할 카드 갯수
    [SerializeField]
    private const int REROLL_COST = 100;    // 리롤 비용 설정

    // 게임 시작 시 처음으로 5장의 카드를 뽑아 리스트로 반환한다.
    public List<CardRawData> GetInitialHand()
    {
        if (ResourceManager.Instance == null)
        {
            Debug.LogError("ResourceManager가 씬에 없습니다!");
            return new List<CardRawData>();   // 빈 리스트를 반환하여 오류 방지.
        }

        if (ResourceManager.Instance.allCards.Count == 0)
        {
            Debug.LogError("ResourceManager에 등록된 카드가 0개입니다!");
            return new List<CardRawData>();   // 빈 카드 목록을 반환하여 오류 방지.  
        }

        currentHandData.Clear();    // 기존 카드들 지우고 새로운 카드를 채움.

        for (int i = 0; i < MAX_SLOTS; i++)
        {
            currentHandData.Add(GetRandomCardData());   // 카드추가
        }
        // 완성된 카드 데이터 리스트를 UIManager등에 전달.
        return currentHandData;
    }

    public CardRawData GetRandomCardData()
    {
        if (ResourceManager.Instance == null)
        {
            return null;
        }
        // 리소스 매니저가 가진 전체 카드 목록을 가져온다.
        var allData = ResourceManager.Instance.allCards;
        return allData[Random.Range(0, allData.Count)];
    }

    // 카드 선택 시 데이터 처리 (ex.비용계산 등)
    //public void ProcessCardSelection(CardRawData data)
    //{
    //    Debug.Log($"[Logic] {data.cardName} 데이터 처리 중...");
    //    // 여기서부터 골드 차감이나 데이터적인 처리를 수행한다.
    //}

    // 리롤을 할 수 있는지 체크하고 골드를 차감하는 논리
    public bool TryProcessReroll()
    {
        // GameManger에게 현재 소지금 확인.
        if(GameManager.Instance.GetCurrentGold() >= REROLL_COST)
        {
            // 골드 차감 실행
            GameManager.Instance.SpendGold(REROLL_COST);

            //리롤 남은 금액
            int remainingGold = GameManager.Instance.GetCurrentGold();

            Debug.Log($"[Logic] 리롤 비용 {REROLL_COST}골드 차감 완료");
            Debug.Log($"[Game] 리롤 완료! 남은 골드: {remainingGold}");

            return true;
        }
        Debug.Log("[Logic] 골드가 부족하여 리롤 불가");
        return false;
    }


}