using System.Collections;
using UnityEngine;
using static EnemyUnit;
using static UnityEngine.GraphicsBuffer;

public class PlayerUnit : MonoBehaviour, IDamageable
{
    public delegate void DeadCallback(PlayerUnit _dead);
    private DeadCallback setDeadCallback = null;
    public DeadCallback SEtDeadCallback
    {
        set { setDeadCallback = value; }
    }
    public enum EPlayerType
    { Player }

    public EPlayerType type = EPlayerType.Player;
    public enum UnitState
    {
        Idle,
        Move,
        Chase,
        Attack,
        Die
    }
    private Vector3[] path;
    private int targetIndex;
    
    private bool isChase = false;
    [SerializeField]
    private Transform target;
    private Camera mainCam;
    public bool shouldMove = false;
    private bool isUnitGrid = false;

    private float normalSpeed = 5f;
    private float chaseSpeed = 7f;
    
    private float detectionRange = 10f;
    private float attackRange = 5f;
    private Vector3 destination;

    private float dmg = 5f;

    private UnitState state = UnitState.Idle;
    private UnitState lastState = UnitState.Idle;
    //private MeshRenderer mr = null;
    private Animator animator;

    private float maxHealth = 100f;
    private float curHealth = 100f;
    public float MaxHealth
    {
        get { return maxHealth; }
    }
    public float CurHealth
    {
        get { return curHealth; }
    }

    [SerializeField]
    private GameObjectList gameObjectList;
    public GameObjectList GameObjectList
    {
        set { gameObjectList = value; }
    }
    private Transform[] playerUnitList;
    private Transform[] enemyUnitList;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void OnEnable()
    {
        curHealth = maxHealth;
    }
    private void Start()
    {
        UnitSelectionManager.Instance.allUnitsList.Add(gameObject);
        StartCoroutine(UpdateList());
        animator = GetComponent<Animator>();
        //mr = transform.GetComponentInChildren<MeshRenderer>();
    }

    private void Update()
    {
        //CheckChase();
        if (Input.GetMouseButtonDown(1) && shouldMove)
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer("Floor")))
            {
                destination = hitInfo.point;
                bool checkisUnit = false;
                PathRequestManager.CheckUnitGrid(hitInfo.point, out checkisUnit);
                Debug.Log("CheckUnitGrid : " + checkisUnit);
                PathRequestManager.RequestPath(transform.position, destination, OnPathFound);

            }
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer("Enemy")))
            {
                target = hitInfo.transform;
            }
        }
    }

    public void OnPathFound(Vector3[] _newpath, bool _pathSuccessful)
    {
        if (_pathSuccessful)
        {
            path = _newpath;

            targetIndex = 0;
            if (path.Length == 0 || path == null) return;
            if (!isChase)
            {
                StopCoroutine("FollowPath");
                //StopCoroutine("Chase");
                StartCoroutine("FollowPath");
            }
        }
        else
        {
            PathRequestManager.RequestPath(transform.position, destination, OnPathFound);
        }
    }
    private IEnumerator FollowPath()
    {
        if (path == null || path.Length == 0)
        {
            state = UnitState.Idle;
            UpdateAnimation(state);
            yield break;
        }
        Vector3 currentWaypoint = path[0];
        float timer = 0f;
        float refreshRate = 0.25f;
        float checkChaseTime = 0.25f;
        state = UnitState.Move;
        while (true)
        {
            UpdateAnimation(state);
            timer += Time.deltaTime;
            if (timer >= checkChaseTime)
            {
                timer = 0f;
                CheckChase();
            }

            timer += Time.deltaTime;
            if (timer >= refreshRate)
            {
                timer = 0f;
                PathRequestManager.RequestPath(transform.position, destination, OnPathFound);
            }

            if (Vector3.Distance(transform.position, currentWaypoint) < 0.1f)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    state = UnitState.Idle;
                    UpdateAnimation(state);
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }
            float speed = isChase ? chaseSpeed : normalSpeed;
            state = UnitState.Move;
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            Turn(transform.position, currentWaypoint);

            yield return null;

        }
    }

    private void Turn(Vector3 _startPos, Vector3 _endPos)
    {
        Vector3 direction = _endPos - _startPos;
        float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 90f - angle, 0f);
    }
    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; ++i)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
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
        foreach (Transform tr in enemyUnitList)
        {
            if (tr == null || !tr.gameObject.activeInHierarchy) continue;
            //float dist = Vector3.Distance(transform.position, tr.position);
            float dist = (transform.position - tr.position).sqrMagnitude;
            if (dist <= detectionRange * detectionRange)
            {
                isChase = true;
                state = UnitState.Chase;
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
        float refreshRate = 0.25f; 
        float timer = 0f;

        state = UnitState.Chase;
        UpdateAnimation(state);
        while (target != null && isChase)
        {
            timer += Time.deltaTime;
            if (timer >= refreshRate)
            {
                timer = 0f;
                if (target != null)
                    PathRequestManager.RequestPath(transform.position, SetTargetPos(target.position), OnPathFound);
            }
            UpdateAnimation(state);
            if (!target.gameObject.activeInHierarchy)
            {
                break;
            }
            float dist = (transform.position - target.position).sqrMagnitude;
            if (dist <= attackRange * attackRange)
            {
                state = UnitState.Attack;
                StartCoroutine("AttackCoroutine");
                //mr.material.color = Color.red;
                yield break;
              
            }
            else if (dist <= detectionRange * detectionRange)
            {
                state = UnitState.Chase;
                //mr.material.color = Color.yellow;
            }
            else
            {
                target = null;
                state = UnitState.Idle;
                isChase = false;
                //mr.material.color = Color.white;

                break;
            }


            if (path != null && path.Length > 0)
            {
                if (targetIndex >= path.Length)
                    targetIndex = path.Length - 1;

                Vector3 currentWaypoint = path[targetIndex];

                if (Vector3.Distance(transform.position, currentWaypoint) < 1f)                 {
                    targetIndex++;
                    if (targetIndex >= path.Length)
                    {
                        targetIndex = path.Length - 1;
                    }
                    currentWaypoint = path[targetIndex];
                }

                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, chaseSpeed * Time.deltaTime);
                Turn(transform.position, currentWaypoint);

            }

            yield return null; // 한 프레임 대기
        }
        isChase = false;
        if (state != UnitState.Attack && state != UnitState.Die)
        {
            state = UnitState.Idle;
            UpdateAnimation(state);
        }
    }

    private void UpdateAnimation(UnitState newState)
    {
        if (state == lastState) return;
        lastState = state;
        animator.SetBool("Idle", false);
        animator.SetBool("Move", false);
        animator.SetBool("Chase", false);
        animator.SetBool("Attack", false);

        switch (newState)
        {
            case UnitState.Idle:
                animator.SetBool("Idle", true);
                break;
            case UnitState.Move:
                animator.SetBool("Move", true); // Move와 Chase를 같은 모션으로 쓸거면 여기서 조정
                break;
            case UnitState.Chase:
                animator.SetBool("Chase", true); // 혹은 "Move"를 켤 수도 있음
                break;
            case UnitState.Attack:
                animator.SetBool("Attack", true);
                break;
        }
    }
    private IEnumerator UpdateList()
    {
        float time = 0;
        float checktime = 0.25f;
        while (true)
        {
            time += Time.deltaTime;
            if (time > checktime)
            {
                time = 0f;
                playerUnitList = gameObjectList.playerUnitList();
                enemyUnitList = gameObjectList.enemyUnitList();
            }
            yield return null;
        }
    }
    private Vector3 SetTargetPos(Vector3 _targetPos)
    {
        for (int x = -1; x <= 1; ++x)
        {
            for (int y = -1; y <= 1; ++y)
            {
                Vector3 checkPos = new Vector3(_targetPos.x + x, _targetPos.y, _targetPos.z + y);
                PathRequestManager.CheckUnitGrid(checkPos, out isUnitGrid);
                if (!isUnitGrid)
                {
                    return checkPos;
                }
            }
        }
        return _targetPos;
    }
    public void TakeDamage(float damage, IDamageable _target)
    {
        if (state == UnitState.Die || curHealth <= 0)
            return;
        curHealth -= (int)damage;
        Debug.Log("Name : " + gameObject.name + ",Hp : " + curHealth);
        if (_target != null)
        {
            if (target == null)
                target = _target.transform;
            //float originTarget = (target.position - transform.position).sqrMagnitude;
            //float newTarget = (_target.transform.position - transform.position).sqrMagnitude;
            //if (originTarget > newTarget)
            //{
            //    target = _target.transform;
            //}
        }
        if (curHealth <= 0f)
        {
            Die();
        }
    }
    private IEnumerator AttackCoroutine()
    {
        StopCoroutine("Chase");
        StopCoroutine("FollowPath");
        float attackDelay = 5f;
        float time = attackDelay;
        isChase = false;
        while (target != null && target.gameObject.activeInHierarchy)
        {
            float dist = (transform.position - target.position).sqrMagnitude;
            if (dist > attackRange * attackRange)
            {
                break;
            }
            time += Time.deltaTime;
            if (time >= attackDelay)
            {
                if (target == null || !target.gameObject.activeInHierarchy) break;
                Turn(transform.position, target.position);
                state = UnitState.Attack;
                UpdateAnimation(state);
                //Debug.Log("TargetName : " + target.name + "(" + dmg + ")");
                IDamageable toTargetDmg = target.GetComponent<IDamageable>();
                if(toTargetDmg != null)
                {
                    toTargetDmg.TakeDamage(dmg, this);
                }
                time = 0f;

                yield return new WaitForSeconds(1f);
                state = UnitState.Idle;
                UpdateAnimation(state);
            }
            yield return null;
        }
        if(state == UnitState.Attack)
        {
            state = UnitState.Idle;
            UpdateAnimation(state);
        }
        if (target != null && target.gameObject.activeInHierarchy)
        {
            float currentDist = (transform.position - target.position).sqrMagnitude;
            if (currentDist <= detectionRange * detectionRange)
                CheckChase();
        }
        else
        {
            target = null;
        }
    }
    private void Die()
    {
        if (state == UnitState.Die)
            return;
        StopAllCoroutines();
        StartCoroutine("DeadCoroutine");
        UnitSelectionManager.Instance.allUnitsList.Remove(gameObject);
        if (UnitSelectionManager.Instance.unitsSelected.Contains(gameObject))
        {
            UnitSelectionManager.Instance.unitsSelected.Remove(gameObject);
        }
    }

    private IEnumerator DeadCoroutine()
    {
        state = UnitState.Die;
        UpdateAnimation(state);

        yield return new WaitForSeconds(2f);
        setDeadCallback?.Invoke(this);
        this.gameObject.SetActive(false);
    }


}
