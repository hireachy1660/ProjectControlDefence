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
    [SerializeField] private RectTransform visualRoot = null;

    private CardRawData myData;     // 카드 정보
    private UIManager uiManager;    // 최상위 매니저 참조

    private LayoutElement layout;
    private CanvasGroup cg;
    private RectTransform rect;
    private bool isFollowing = false;
    private bool isDestroying = false;

    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        // 시작할 때 visualRoot가 엉뚱한 데 있지 않게 초기화
        if (visualRoot != null) visualRoot.anchoredPosition = Vector2.zero;
    }
    // UICard 내부 또는 별도 스크립트
    void Update()
    {
        if (!isFollowing || visualRoot == null) return;

        // [핵심] 알맹이가 부모(Card_Btn)의 위치를 부드럽게 쫓아감 (슬라이드 연출)
        // World Position을 기준으로 쫓아가면 레이아웃이 밀릴 때 스윽~ 하고 움직입니다.
        visualRoot.position = Vector3.Lerp(visualRoot.position, transform.position, Time.deltaTime * 12f);

        // 투명도도 부드럽게 채움
        cg.alpha = Mathf.Lerp(cg.alpha, 1, Time.deltaTime * 5f);
    }

    // 데이터가 들어오면 UI를 갱신하는 입구
    public void Setup(CardRawData data, UIManager manager, bool isReroll = false)
    {
        myData = data;  // 전달받은 데이터를 내 저장소에 저장
        uiManager = manager;

        nameText.text = data.cardName;          // UI에 이름표시
        costText.text = data.cost.ToString();   // 숫자인 코스트를 문자로 바꿔서 표시
        iconImage.sprite = data.cardIcon;       // 데이터 속 이미지

        // 버튼 중복 클릭 방지 루틴
        Button btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => {
            if (!isDestroying) manager.OnCardClicked(data, this);
        });

        // 생성이 완료되면 쫓아가기 시작
        isFollowing = true;
        isDestroying = false;
        cg.alpha = 0; // 페이드 인 준비
    }
    // 카드 클릭 되었을 때 내가 눌렀어라고 UIManager에게 알리는 역할.


    // 애니메이션 UI영역이므로 여기에 위치
    // 선택된 카드가 좁아지며 사라지는 애니메이션 - 축소 및 파괴 함수
    public IEnumerator Co_ShrinkAndDestroy(float duration)
    {
        cg.blocksRaycasts = false; // 삭제 중 클릭 방지
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = 1 - (elapsed / duration);
            yield return null;
                
            }

            Destroy(gameObject);    // 애니메이션이 끝나면 오브젝트 파괴
        }

    // [핵심] 이 함수가 옆 카드들을 부드럽게 옮겨줍니다.
    public IEnumerator Co_MoveLeft(float distance, float duration)
    {
        float elapsed = 0;
        // 현재 위치(보통 0,0)에서 왼쪽으로 밀기
        Vector2 startPos = visualRoot.anchoredPosition;
        Vector2 targetPos = startPos + new Vector2(-distance, 0);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // 부드러운 곡선 적용 (중급자 느낌 살짝 추가)
            float curve = Mathf.SmoothStep(0, 1, t);

            visualRoot.anchoredPosition = Vector2.Lerp(startPos, targetPos, curve);
            yield return null;
        }

        // 이동이 끝나면 좌표를 초기화하고 실제 레이아웃 위치로 고정
        visualRoot.anchoredPosition = Vector2.zero;
    }
    public void SetOffset(float distance)
    {
        // visualRoot를 오른쪽으로 밀어두어 화면 밖에서 대기하게 함
        visualRoot.anchoredPosition = new Vector2(distance, 0);
    }
    public void OutAndDestroy()
    {
        // 삭제될 때는 쫓아가기를 멈추고 파괴
        isFollowing = false;
        Destroy(gameObject);
    }
    // 카드가 사라질 때 (공간은 유지하고 비주얼만 페이드아웃)
    public IEnumerator Co_FadeOutAndHide(float duration)
    {
        isDestroying = true;
        cg.blocksRaycasts = false;
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = 1 - (elapsed / duration);
            yield return null;
        }
        // 바로 파괴하지 않고 UIManager가 파괴하도록 대기 (공간 유지 때문)
    }
}
