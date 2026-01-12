using UnityEngine;
using System.Collections;

public class EnemyUnit : MonoBehaviour, IDamageable
{
    public enum EnemyState
    {
        Idle,
        Move,
        Chase,
        Attack,
        Die
    }
    //경로 탐색
    private Vector3[] path;
    private int targetIndex; // path[]를 위한 index
    private bool isUnitGrid = false;

    // 유닛 상태
    private bool isChase = false;
    private Transform target; // 추적 대상

    private float normalSpeed = 5f;
    private float chaseSpeed = 7f;

    private float detectionRange = 10f;
    private float attackRange = 2.5f;
    private  float attackDelay = 10f;
    private float attackTime = 0f;
    
    private float dmg = 5f;

    private EnemyState state = EnemyState.Idle;
    private EnemyState lastState = EnemyState.Idle;
    //private MeshRenderer mr = null;
    private Animator animator;
    [SerializeField]
    private float maxHealth = 100f;
    private float curHealth = 100f;

    public float MaxHealth
    { get { return maxHealth; } }
    public float CurHealth
    { get { return curHealth; } }

    // 목적지
    [SerializeField]
    private Transform baseCamp;
    private Vector3 baseCampPos;

    // 다른 객체 List를 갖기 위해 GameObjectList를 참조
    [SerializeField]
    private GameObjectList gameObjectList;
    [SerializeField]
    private Transform[] playerUnitList;
    [SerializeField]
    private Transform[] towerUnitList;
    private Transform[] enemyUnitList;
    
    // 같은 유닛끼리의 격리를 위한 변수(보류)
    private float separateRange = 1.5f;
    private float separateForce = 15f;


    [Header("충돌 감지 설정")]
    public LayerMask unitLayer;
    public float stopDistance = 2f; // 앞 유닛과의 안전 거리

    private bool IsBlockedByUnit(Vector3 targetPosition)
    {
        
        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 rayStart = transform.position + Vector3.up * 2f;
        Vector3 rayEnd = direction * stopDistance;
       
        Debug.DrawRay(rayStart, rayEnd, Color.green);

        if (Physics.Raycast(rayStart, direction, out RaycastHit hit, stopDistance, unitLayer))
        {
            if (hit.transform != this.transform)
            {
                Debug.DrawLine(rayStart, hit.point, Color.red);
                return true;
            }
        }
        return false;
    }

    private void OnEnable()
    {
        curHealth = maxHealth;
    }

    private void Start()
    {
        baseCampPos = baseCamp.position;
        StartCoroutine(UpdateList()); // 리스트를 최신화
       
        PathRequestManager.RequestPath(transform.position, baseCampPos, OnPathFound);

        //mr = transform.GetComponentInChildren<MeshRenderer>();
        animator = GetComponent<Animator>();
    }

    public void OnPathFound(Vector3[] _newpath, bool _pathSuccessful)
    {
        if(_pathSuccessful)
        {
            path = _newpath;
           
            targetIndex = 0;
            if (path.Length == 0 || path == null) return;
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
        state = EnemyState.Move;
        while(true)
        {
            UpdateAnimation(state);
            time += Time.deltaTime;
            if (time >= checkChaseTime) // 길찾기 수행 중 추적범위에 따른 상태 변화
            {
                time = 0f;
                CheckChase();
            }

            if (Vector3.Distance(transform.position,baseCampPos) <= 10f)
            {
                float endspeed = isChase ? chaseSpeed : normalSpeed;
                if (IsBlockedByUnit(currentWaypoint))
                {
                    state = EnemyState.Idle;
                }
                else
                {
                    state = EnemyState.Move;
                    transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, endspeed * Time.deltaTime);
                    Turn(transform.position, currentWaypoint);
                }
                if (Vector3.Distance(transform.position, currentWaypoint) < 2f)
                    yield break;

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
            if (IsBlockedByUnit(currentWaypoint))
            {
                state = EnemyState.Idle;
            }
            else
            {
                state = EnemyState.Move;
                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
                Turn(transform.position, currentWaypoint);
            }
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
        if (playerUnitList == null || towerUnitList == null) return;
        foreach(Transform tr in towerUnitList)
        {
            if (tr == null || !tr.gameObject.activeInHierarchy) continue;
            float dist = (transform.position - tr.position).sqrMagnitude;
            if (dist <= detectionRange * detectionRange)
            {
                isChase = true;
                state = EnemyState.Chase;
                target = tr;
                StopCoroutine("FollowPath");
                StopCoroutine("Attack");
                StartCoroutine("Chase");
                break;
            }
           
        }
        foreach (Transform tr in playerUnitList)
        {
            if (tr == null || !tr.gameObject.activeInHierarchy) continue;
            float dist = (transform.position - tr.position).sqrMagnitude;
            if (dist <= detectionRange * detectionRange)
            {
                isChase = true;
                state = EnemyState.Chase;
                target = tr;
                StopCoroutine("FollowPath");
                StopCoroutine("Attack");
                StartCoroutine("Chase");
                break;
            }

        }
    }

    private IEnumerator Chase()
    {
        float refreshRate = 0.25f; // 경로 갱신 주기 (0.25초마다 경로 재계산)
        float timer = 0f;

        while (target != null && isChase)
        {
            timer += Time.deltaTime;
            if (timer >= refreshRate)
            {
                timer = 0f;
                PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
            }
            UpdateAnimation(state);
            if (!target.gameObject.activeInHierarchy)
            {
                break;
            }
            float dist = (transform.position - target.position).sqrMagnitude;
            if (dist <= attackRange * attackRange)
            {
                state = EnemyState.Attack;
                StartCoroutine("AttackCoroutine");
                yield break;
            }
            else if(dist <= detectionRange * detectionRange)
            {
                state = EnemyState.Chase;
            }
            else
            {
                target = null;
                state = EnemyState.Move;
                break;
            }

            if (path != null && path.Length > 0)
            {
                if (targetIndex >= path.Length)
                    targetIndex = path.Length - 1;

                Vector3 currentWaypoint = path[targetIndex];
                if (Vector3.Distance(transform.position, currentWaypoint) < 1f)               {
                    targetIndex++;
                    if (targetIndex >= path.Length)
                    {
                        targetIndex = path.Length - 1;
                    }
                    currentWaypoint = path[targetIndex];
                }
                if (IsBlockedByUnit(currentWaypoint))
                {
                    state = EnemyState.Idle;
                }
                else
                {
                    state = EnemyState.Chase;
                    transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, chaseSpeed * Time.deltaTime);
                    Turn(transform.position, currentWaypoint);
                }
            }

            yield return null;
        }
        isChase = false;
        PathRequestManager.RequestPath(transform.position, baseCampPos, OnPathFound);
    }

    private void UpdateAnimation(EnemyState newState)
    {
        if (state == lastState) return;
        lastState = state;
        animator.SetBool("Idle", false);
        animator.SetBool("Move", false);
        animator.SetBool("Chase", false);
        animator.SetBool("Attack", false);

        switch (newState)
        {
            case EnemyState.Idle:
                animator.SetBool("Idle", true);
                break;
            case EnemyState.Move:
                animator.SetBool("Move", true); 
                break;
            case EnemyState.Chase:
                animator.SetBool("Chase", true);
                break;
            case EnemyState.Attack:
                animator.SetBool("Attack", true);
                break;
        }
    }

    private IEnumerator UpdateList()
    {
        float time = 0;
        float checktime = 0.25f;
        while(true)
        {
            time += Time.deltaTime;
            if(time > checktime)
            {
                time = 0f;
                playerUnitList = gameObjectList.playerUnitList();
                enemyUnitList = gameObjectList.enemyUnitList();
                towerUnitList = gameObjectList.towerUnitList();
            }
            yield return null;
        }
    }
    public void TakeDamage(float damage) 
    {
        curHealth -= (int)damage;
        Debug.Log("Name : " + gameObject.name + ",Hp : "  + curHealth);
    }

    private IEnumerator AttackCoroutine()
    {
        StopCoroutine("Chase");
        StopCoroutine("FollowPath");
        float attackDelay = 5f;
        float time = attackDelay;
        float dist = 0f;
        isChase = false;
        while(true)
        {
            dist = (transform.position - target.position).sqrMagnitude;
            time += Time.deltaTime;
            if(time>=attackDelay)
            {

                state = EnemyState.Attack;
                Turn(transform.position, target.position);
                Debug.Log("TargetName : " + target.name + "(" + dmg + ")");
                time = 0f;
                UpdateAnimation(state);

                yield return new WaitForSeconds(1f);
                continue;
            }
            state = EnemyState.Idle;
            UpdateAnimation(state);
            if (dist > attackRange * attackRange)
            {
                break;
            }
            yield return null;
        }
        if (dist > detectionRange * detectionRange)
        {
            target = null;
            PathRequestManager.RequestPath(transform.position, baseCampPos, OnPathFound);
        }
        else
        {
            CheckChase();
        }
    }
}
