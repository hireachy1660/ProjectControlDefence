using UnityEngine;
using System.Collections;   //코루틴
public class CameraScroller : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float scrollSpeed = 25f;
    [SerializeField] private float edgeSize = 0.03f;     // 화면 비율(3%) 계산, (3%~5% 적당)

    // 카메라가 이동 가능한 x,z축의 최소/최대 좌표
    // 카메라가 맵 밖으로 탈출하는것을 방지
    [Header("Map Boundary")]
    [SerializeField] private Vector2 minBounds;     // 좌표는 맵좌표에서 확인(x 가로, y 세로(z축))
    [SerializeField] private Vector2 maxBounds;

    [Header("Targeting & Spacebar")]
    [SerializeField] private Transform lastSelectedUnit = null; // 최근 선택된 유닛
    [SerializeField] private float lerpSpeed = 20f;              // 화면 이동 부드러움 정도
    [SerializeField] private float cameraDistance = 20f;        // 캐릭터를 중앙에 두기 위한 후퇴 거리

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 20f;

    [Header("Intro Cinematic")]
    [SerializeField] private bool playIntro = true;     //인트로 연출을 켤지 말지
    [SerializeField] private Vector3[] introWaypoints;  //거처갈 지점들( x,y 좌표 위주)
    [SerializeField] private float introSpeed = 2f;     //인트로 이동속도
    [SerializeField] private float waitAPoint = 0.5f;   //각 지점에 도착 후 머무르는 시간

    private Camera camComponent;
    private bool isFollowing = false;
    private bool isIntroPlaying = false;                // 인트로 재생 중에서 마우스 조작 금지

    private void Awake()
    {
        camComponent = GetComponent<Camera>();
    }

    private void Start()
    {
        if(playIntro)
        {
            StartCoroutine(PlayIntroCinematic());
        }
        else
        {
            // 인트로 안할 떄는 바로 원점 세팅
            SetDefaultPosition();
        }

            // 시작 시 원점(0,0,0) 근처를 비추도록 카메라 초기 위치 설정
            // 카메라 높이기 20이면 (-20, 20, -20) 위치가 원점을 정중앙에 비춘다.
            //float startHeight = transform.position.y;
        //transform.position = new Vector3(-startHeight, startHeight, -startHeight);
    }

    //Update보다 화면 떨림이 적은 LateUpdate사용
    private void LateUpdate()
    {
        if (isIntroPlaying) return; // 인트로 재생 중 아래 조작 무시!

        HandleZoom();

        // 최근 선택 유닛으로 이동(스페이스바)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 스페이스바를 누르면 최근유닛에게 따라가는 코드
            if (lastSelectedUnit != null)
            {
                isFollowing = true;
            }
        }
        // 이동 로직 결정
        // 추적 중일 때는 마우스 이동 무시, 마우스로 화면을 움직이려 하면 추적모드 자동끄기.
        if (isFollowing)
        {
            //FollowTarget();
        }
        else
        {
            HandleEdgeScrolling();
        }
    }

    //
    // 스크린 엣지 마우스 컨트롤 로직
    private void HandleEdgeScrolling()
    {
        Vector3 moveDir = Vector3.zero;     // 마우스 위치에 따라 어느 방향으로 갈지 정함(오른쪽 x=1, 왼쪽 x=-1)
        Vector3 mousePos = Input.mousePosition;

        // 화면 비율 기반 감지( 해상도 대응 )
        // Screen.width * (1 - edgeSize) : 화면 오른쪽 끝에서 3% 안쪽 지점의 좌표를 구한다.
        if (mousePos.x >= Screen.width * (1 - edgeSize)) moveDir.x += 1;    
        if (mousePos.x <= Screen.width * edgeSize) moveDir.x -= 1;
        if (mousePos.y >= Screen.height * (1 - edgeSize)) moveDir.z += 1;
        if (mousePos.y <= Screen.height * edgeSize) moveDir.z -= 1;

        if(moveDir != Vector3.zero)
        {
            isFollowing = false; // 마우스 움직이면 추적 모드 해제
            Move(moveDir.normalized);   // 대각선 이동 시 속도가 빨라지지 않게 벡터 정규화
        }
    }

    // 아이소매트릭 이동 핵심
    private void Move(Vector3 direction)
    {
        // 쿼터뷰(45도) 이동 보정
        // 쿼터뷰 대각선 방향을 월드 좌표와 일치
        // 아이소매트릭에서 '화면위'는 실제 월드(1,0,1) 방향
        Vector3 forward = new Vector3(1, 0, 1).normalized;  // 아이소매트릭 위쪽
        Vector3 right = new Vector3(1, 0, -1).normalized;   // 아이소매트릭 오른쪽, 화면상 오른쪽으로 x와 -z 섞은 새로운 축 재정의

        Vector3 targetPos = transform.position + (right * direction.x + forward * direction.z) * scrollSpeed * Time.deltaTime;

        // 맵 범위 제한 후 적용
        transform.position = ClampPosition(targetPos);

    }

    // 부드럽게 유닛에게 다가가기
    //private void FollowTarget()
    //{
    //    if (lastSelectedUnit == null) return;

    //    // 유닛의 위치를 유지하며 카메라 높이(Y)와 각도 보정값 계산
    //    // (카메라 Rig 기준이므로 offset을 고려하거나 단순 Lerp 사용 )
    //    // 핵심, 캐릭터를 화면 중앙에 오는 공식
    //    // 유닛의 발밑 좌표에서 카메라 각도만큼 뒤로 뺀 좌표가 목표 지점이다.
    //    Vector3 targetPos = new Vector3(lastSelectedUnit.position.x - cameraDistance, transform.position.y, lastSelectedUnit.position.z - cameraDistance);

    //    // 부드럽게 선형보간
    //    transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * lerpSpeed);

    //    // 거의 도착하면 추적 중단( 원할 경우 계속 추적하려면 조건 제거 )
    //    // 유닛 위치에 거의 다 도착했는지 확인하고, 도착했다면 자동으로 추적 모드 해제
    //    if (Vector3.Distance(transform.position, targetPos) < 0.1f && !Input.GetKey(KeyCode.Space)) 
    //    {
    //        isFollowing = false;
    //    }
    //}

    // 경계 제한 - 카메라 맵 밖 '공허'를 보여주지 않는다.
    private Vector3 ClampPosition(Vector3 pos)
    {
        return new Vector3(Mathf.Clamp(pos.x, minBounds.x, maxBounds.x), pos.y, Mathf.Clamp(pos.z, minBounds.y, maxBounds.y));
    }

    // 외부( 유닛 선택 스크립트 )에서 유닛을 등록할 때 호출하는 함수
    public void SetTarget(Transform unit)
    {
        lastSelectedUnit = unit;
    }
    // 마우스 휠로 화면 확대 축소
    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if(scroll != 0)
        {
            if(camComponent.orthographic)
            {
                float newSize = camComponent.orthographicSize - (scroll * zoomSpeed);
                camComponent.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
            }
        }
    }
    private IEnumerator PlayIntroCinematic()
    {
        // 예외처리 : 웨이포인트 없으면 종료
        if (introWaypoints == null || introWaypoints.Length == 0) yield break;

        isIntroPlaying = true;

        // 루프를 돌며 모든 지점 순차 방문
        for (int i = 0; i < introWaypoints.Length; i++) 
        {
            Vector3 targetPos = CalculateCameraPos(introWaypoints[i]);

            // Vector3.Distance를 사용해 목적지까지 거리체크
            while (Vector3.Distance(transform.position, targetPos) > 0.1f)
            {
                // MoveTowards는 고정된 이동 표현으로 사용
                transform.position = Vector3.MoveTowards(transform.position, targetPos, introSpeed * Time.deltaTime);
                yield return null;
            }
            // 각 거점에 도착할 때마다 잠깐 멈춰서 보여주기
            if (waitAPoint > 0) yield return new WaitForSeconds(waitAPoint);
        }

        //// 인트로 종료 후 캐릭터 조작
        //isIntroPlaying = false;

        //// 카드 생성
        //FindAnyObjectByType<CardManager>().StartCardGame();
    }
    // 입력받은 x,z 좌표를 카메라 오프셋이 적용된 실제 좌표로 계산해주는 함수
    private Vector3 CalculateCameraPos(Vector3 mapPos)
    {
        // cameraDistance는 캐릭터 중앙 정렬을 위해 쓴 변수
        return new Vector3(mapPos.x - cameraDistance, transform.position.y, mapPos.z - cameraDistance);
    }
    private void SetDefaultPosition()
    {
        transform.position = CalculateCameraPos(Vector3.zero);
    }


}