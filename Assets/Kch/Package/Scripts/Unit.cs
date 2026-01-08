using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Move,
        Chase,
        Attack,
        Die
    }
    private float normalSpeed = 5f;
    private Vector3[] path;
    private int targetIndex;

    private bool isChase = false;

    [Header("거리 및 속도 설정")]
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float chaseSpeed = 10f;

   

    [SerializeField]
    private Transform baseCamp;
    private Vector3 baseCampPos;

    [SerializeField]
    private GameObjectList gameObjectList;
    private Transform[] playerUnitList;
    private Transform target;

    private EnemyState state = EnemyState.Idle;
    private MeshRenderer mr = null;

    private Transform[] enemyUnitList;
    private float separateRange = 1.5f;
    private float separateForce = 15f;
    private void Start()
    {
        playerUnitList = gameObjectList.playerUnitList();
        enemyUnitList = gameObjectList.EnemyUnitList();
        baseCampPos = RandomTargetPosition();
        PathRequestManager.RequestPath(transform.position, baseCampPos, OnPathFound);

        mr = transform.GetComponentInChildren<MeshRenderer>();
    }
    private Vector3 RandomTargetPosition() // 같은 목적지를 받을 경우 뭉치는 현상이 발생하기 때문에 분산시키기 위해 offset을 넣어준다.
    {
        Vector3 randomoffset = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
        return baseCamp.position + randomoffset;
    }
    public void OnPathFound(Vector3[] _newpath, bool _pathSuccessful)
    {
        if(_pathSuccessful)
        {
            path = _newpath;
           
            targetIndex = 0;
            if (!isChase)
            {
                StopCoroutine("FollowPath");
                StartCoroutine("FollowPath");
            }
            
        }
        else
        {
            PathRequestManager.RequestPath(transform.position, baseCampPos, OnPathFound);
        }
    }
    private IEnumerator FollowPath()
    {
        if (path == null || path.Length == 0) yield break;
        Vector3 currentWaypoint = path[0];
        float time = 0f;
        float checkChaseTime = 0.25f;
        while(true)
        {
            time += Time.deltaTime;
            if (time >= checkChaseTime)
            {
                time = 0f;
                CheckChase();
            }

            if (Vector3.Distance(transform.position,baseCampPos) <= 5f)
            {
                state = EnemyState.Idle;
                mr.material.color = Color.cyan;
                yield return null;
                continue;
            }
            if(Vector3.Distance(transform.position,currentWaypoint)<0.1f)
            {
                targetIndex++;
                if(targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }
            float speed = isChase ? chaseSpeed : normalSpeed;
            state = EnemyState.Move;
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            Vector3 separationVector = GetSeparationVector();
            transform.position += separationVector * separateForce * Time.deltaTime;
            Turn(transform.position, currentWaypoint);
            //if (currentWaypoint == path[^1] && Vector3.Distance(transform.position, target.position) <= minTargetDistance)
            //{

            //    yield break;
            //}
           

            yield return null;

            
        }

    }

    private void Turn(Vector3 _startPos, Vector3 _endPos)
    {
        Vector3 direction = _endPos - _startPos;
        float angle = Mathf.Atan2(direction.z, direction.x)*Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 90f - angle, 0f);
    }
    public void OnDrawGizmos()
    {
        if(path != null)
        {
            for (int i = targetIndex; i <path.Length; ++i)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if(i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);

                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }

    private void CheckChase()
    {
        if (playerUnitList == null) return;
        foreach(Transform tr in playerUnitList)
        {
            if (tr == null || !tr.gameObject.activeInHierarchy) continue;
            //float dist = Vector3.Distance(transform.position, tr.position);
            float dist = (transform.position - tr.position).sqrMagnitude;
            if (dist <= detectionRange * detectionRange)
            {
                isChase = true;
                state = EnemyState.Chase;
                target = tr;
                StopCoroutine("FollowPath");
                StartCoroutine("Chase");
                break;
            }
           
        }
    }

    private IEnumerator Chase()
    {
        float refreshRate = 0.25f; // 경로 갱신 주기 (0.25초마다 경로 재계산)
        float timer = 0f;

        // 타겟이 존재하고 추격 상태인 동안 무한 루프
        while (target != null && isChase)
        {
            Vector3 separationVector = GetSeparationVector();
            if (!target.gameObject.activeInHierarchy)
            {
               
                break;
            }
            // 1. 거리 체크 및 공격 (옵션)
            //float dist = Vector3.Distance(transform.position, target.position);
            float dist = (transform.position - target.position).sqrMagnitude;
            if (dist <= attackRange * attackRange)
            {
                state = EnemyState.Attack;
                // 공격 사거리에 들어왔으므로 이동 멈춤 or 공격 로직 수행
                // 여기서는 예시로 이동만 멈추고 대기
               
                mr.material.color = Color.red;
                yield return new WaitForSeconds(1f); // 데미지 처리하는 곳에서 Update를 한번 돌릴 수 있도록 대기 시간을 가지는 것
                continue;
            }
            else if(dist <= detectionRange * detectionRange)
            {
                state = EnemyState.Chase;
                mr.material.color = Color.yellow;
            }
            else
            {
                target = null;
                state = EnemyState.Move;
                mr.material.color = Color.white;
                break;
            }

                // 2. 주기적으로 경로 갱신 요청
                timer += Time.deltaTime;
            if (timer >= refreshRate)
            {
                timer = 0f;
                // 현재 위치에서 타겟의 현재 위치로 경로 요청
                PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
            }

            // 3. 이동 로직 (FollowPath와 유사하지만 while 루프 내부에 존재)
            if (path != null && path.Length > 0)
            {
                // 경로 갱신 시 targetIndex가 배열 범위를 넘는 오류 방지
                if (targetIndex >= path.Length) targetIndex = 0;

                Vector3 currentWaypoint = path[targetIndex];

                // 웨이포인트 도달 확인
                if (Vector3.Distance(transform.position, currentWaypoint) < 0.1f) // 정확한 비교(==)보다 거리 체크가 안전
                {
                    targetIndex++;
                    // 경로 끝에 도달했으면 인덱스 유지 (다음 경로 갱신 기다림)
                    if (targetIndex >= path.Length)
                    {
                        targetIndex = path.Length - 1;
                        //yield break;
                    }
                    currentWaypoint = path[targetIndex];
                }

                // 실제 이동
                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, chaseSpeed * Time.deltaTime);
                //Vector3 separationVector = GetSeparationVector();
                transform.position += separationVector * separateForce * Time.deltaTime;
                Turn(transform.position, currentWaypoint);
                
            }

            yield return null; // 한 프레임 대기
        }
        isChase = false;
        PathRequestManager.RequestPath(transform.position, baseCampPos, OnPathFound);
    }

    //분리 알고리즘
    private Vector3 GetSeparationVector()
    {
        Vector3 separationVector = Vector3.zero;
        float separateSqrMagnitude = separateRange * separateRange;
        foreach (Transform tr in enemyUnitList)
        {
            if (tr == this.transform || tr == null || !tr.gameObject.activeInHierarchy) continue;
            Vector3 direction = transform.position - tr.position;
            float dist = direction.sqrMagnitude;
            if (dist <= separateSqrMagnitude)
            {
                separationVector += direction.normalized / (dist + 0.01f);
            }
        }
        return separationVector;
    }
}
