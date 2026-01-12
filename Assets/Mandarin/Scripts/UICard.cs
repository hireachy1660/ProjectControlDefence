using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

// 개별 UI 표현 및 동작 - 유닛 단위 UI 스크립트, 클릭이벤트, 애니메이션등 보여지는것에 집중
public class UICard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText = null;
    [SerializeField] private TextMeshProUGUI costText = null;
    [SerializeField] private Image iconImage = null;
   
    private CardRawData myData;     // 카드 정보
    private UICardManager uiManager;    // 최상위 매니저 참조

    // 데이터가 들어오면 UI를 갱신하는 입구
    public void Setup(CardRawData data, UICardManager manager)
    {
        myData = data;  // 전달받은 데이터를 내 저장소에 저장
        uiManager = manager;

        nameText.text = data.cardName;          // UI에 이름표시
        costText.text = data.cost.ToString();   // 숫자인 코스트를 문자로 바꿔서 표시
        iconImage.sprite = data.cardIcon;       // 데이터 속 이미지

        // 버튼 중복 클릭 방지 및 초기화
        // AddListener : 버튼 컴포턴트를 찾아 클릭되면 OnClickCard함수 실행 예약 코드
        Button btn = GetComponent<Button>();
        btn.interactable = true;
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(HandleClick);

        // 등장 시 페이드 인 효과(선택사항)
        if(TryGetComponent<CanvasGroup>(out CanvasGroup cg))
        {
            cg.alpha = 0;
            StartCoroutine(FadeIn(cg));
        }
      
    }
    // 카드 클릭 되었을 때 내가 눌렀어라고 UIManager에게 알리는 역할.
    private void HandleClick()
    {
        GetComponent<Button>().interactable = false;    // 중복 클릭 방지 위해 비활성화
        uiManager.OnCardClicked(myData, this);          // 최상위 매니저에게 클릭 보고, 객체 자신을 넘김
    }   
    // 애니메이션 UI영역이므로 여기에 위치
    // 선택된 카드가 좁아지며 사라지는 애니메이션 - 축소 및 파괴 함수
    public IEnumerator Co_ShrinkAndDestroy(float duration)
    {
        LayoutElement layout = GetComponent<LayoutElement>();
        CanvasGroup cg = GetComponent<CanvasGroup>();

        // 현재 실제 너비를 시작점으로 잡기( 고정값 210f보다 안전함 )
        float startWidth = layout.preferredWidth;    
        float elapsed = 0;

        // 중복 계산 방지를 위해 유연한 너비값을 미리 0으로 셋팅
        if(layout != null)
        {
            layout.flexibleWidth = 0;
            layout.minWidth = 0;
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / duration;

            //float smoothPercent = Mathf.Lerp(0, 1, percent);
            //float smoothPrecent = Mathf.Sin(percent * Mathf.PI * 0.5f);

            // EaseOut 큐빅 공식을 사용하면 끝으로 갈수록 부드러워진다.
            float smoothPercent = 1 - Mathf.Pow(1 - percent, 3);

            if (layout != null)
            {
                // 너비를 줄여서 옆 카드들을 당김
                layout.preferredWidth = Mathf.Lerp(startWidth, 0, smoothPercent);
            }
            if(cg != null)
            {
                // 투명도 줄이기
                cg.alpha = 1 - smoothPercent;
            }
            yield return null;  // 다음 프레임 대기
        }
        Destroy(gameObject);    // 애니메이션이 끝나면 오브젝트 파괴
    }
    // 페이드 인 함수
    private IEnumerator FadeIn(CanvasGroup cg)
    {
        float elasped = 0;
        while (elasped < 0.2f)
        {
            elasped += Time.deltaTime;
            cg.alpha = elasped / 0.2f;
            yield return null;
        }
        cg.alpha = 1;
    }

}
