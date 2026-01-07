using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Cards : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText = null;
    [SerializeField] private TextMeshProUGUI costText = null;
    [SerializeField] private Image iconImage = null;

    // 임시, 이 카드가 어떤정보인지(이름, 가격, 이미지등) - ***수정
    private CardRawData myData;

    // 데이터가 들어오면 UI를 갱신하는 입구
    public void Setup(CardRawData data)
    {
        myData = data;  // 전달받은 데이터를 내 저장소에 저장
        nameText.text = data.cardName;  // UI에 이름표시
        costText.text = data.cost.ToString();   // 숫자인 코스트를 문자로 바꿔서 표시
        iconImage.sprite = data.cardIcon;   // 데이터 속 이미지

        // 버튼 클릭 이벤트도 스스로 등록
        // AddListener : 버튼 컴포턴트를 찾아 클릭되면 OnClickCard함수 실행 예약 코드
        GetComponent<Button>().onClick.RemoveAllListeners();    // 기존 예약 취소 - 중복 방지
        GetComponent<Button>().onClick.AddListener(OnClickCard);// 예약
    }
    private void OnClickCard()
    {
        // 클릭되면 매니저에게 저(gameObject) 클릭됐어요 전달
        CardManager.Instance.OnCardSelected(myData, gameObject);
    }
}
