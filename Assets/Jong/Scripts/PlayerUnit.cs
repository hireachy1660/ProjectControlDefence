using System.Collections;
using UnityEngine;
using static EnemyUnit;
using static UnityEngine.GraphicsBuffer;

public class PlayerUnit : MonoBehaviour
{
    public enum UnitState
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
    public float detectionRange = 10f;
    public float attackRange = 5f;
    public float chaseSpeed = 10f;


    [SerializeField]
    private GameObjectList gameObjectList;
    private Transform[] playerUnitList;
    private Vector3 destination;

    //private UnitState state = UnitState.Idle;
    private MeshRenderer mr = null;

    private Transform[] enemyUnitList;
    [SerializeField]
    private Transform target;

    //private Animator animator;
    private Camera mainCam;
    public bool shouldMove = false;
    private bool isUnitGrid = false;
    private void Awake()
    {
        mainCam = Camera.main;
    }
    private void Start()
    {
        UnitSelectionManager.Instance.allUnitsList.Add(gameObject);
        StartCoroutine(UpdateList());
        
        mr = transform.GetComponentInChildren<MeshRenderer>();
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
                bool checkisUnit = false;
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
            
            StopCoroutine("FollowPath");
            StopCoroutine("Chase");
            StartCoroutine("FollowPath");
        }
        else
        {
            PathRequestManager.RequestPath(transform.position, destination, OnPathFound);
        }
    }
    private IEnumerator FollowPath()
    {
        if (path == null || path.Length == 0) yield break;
        Vector3 currentWaypoint = path[0];
        float timer = 0f;
        float refreshRate = 0.25f;
        float checkChaseTime = 0.25f;
        //state = UnitState.Move;
        //UpdateAnimation(state);
        while (true)
        {
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
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }
            float speed = isChase ? chaseSpeed : normalSpeed;
            //state = UnitState.Move;
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
                //state = UnitState.Chase;
                target = tr;
                StopCoroutine("FollowPath");
                StartCoroutine("Chase");
                break;
            }

        }
    }
    private IEnumerator Chase()
    {
        float refreshRate = 0.25f; 
        float timer = 0f;

        while (target != null && isChase)
        {
            if (!target.gameObject.activeInHierarchy)
            {
                break;
            }
            float dist = (transform.position - target.position).sqrMagnitude;
            if (dist <= attackRange * attackRange)
            {
                StartCoroutine("AttackCoroutine");
                mr.material.color = Color.red;
                //UpdateAnimation(state);
                yield break;
              
            }
            else if (dist <= detectionRange * detectionRange)
            {
                //state = EnemyState.Chase;
                mr.material.color = Color.yellow;
                //UpdateAnimation(state);
            }
            else
            {
                target = null;
               //state = EnemyState.Move;
                mr.material.color = Color.white;
                //UpdateAnimation(state);

                break;
            }

            timer += Time.deltaTime;
            if (timer >= refreshRate)
            {
                timer = 0f;
                PathRequestManager.RequestPath(transform.position, SetTargetPos(target.position), OnPathFound);
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
                //PathRequestManager.CheckUnitGrid(currentWaypoint, out isUnitGrid);
                //if (isUnitGrid)
                //{
                //    currentWaypoint -= (transform.position - currentWaypoint) * 2f;
                //}
                Turn(transform.position, currentWaypoint);

            }

            yield return null; // 한 프레임 대기
        }
        isChase = false;
    }
    
    //private void UpdateAnimation(UnitState newState)
    //{
    //    animator.SetBool("Idle", false);
    //    animator.SetBool("Move", false);
    //    animator.SetBool("Chase", false);
    //    animator.SetBool("Attack", false);

    //    switch (newState)
    //    {
    //        case UnitState.Idle:
    //            animator.SetBool("Idle", true);
    //            break;
    //        case UnitState.Move:
    //            animator.SetBool("Move", true); // Move와 Chase를 같은 모션으로 쓸거면 여기서 조정
    //            break;
    //        case UnitState.Chase:
    //            animator.SetBool("Chase", true); // 혹은 "Move"를 켤 수도 있음
    //            break;
    //        case UnitState.Attack:
    //            animator.SetBool("Attack", true);
    //            break;
    //    }
    //}
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
        Vector3 targetPos = _targetPos;
        for (int x = -2; x <= 2; ++x)
        {
            for (int y = -2; y <= 2; ++y)
            {
                PathRequestManager.CheckUnitGrid(new Vector3(_targetPos.x + x, 0f, _targetPos.z + y), out isUnitGrid);
                if (!isUnitGrid)
                {
                    return targetPos - (transform.position - targetPos);
                }
            }
        }

        return targetPos - (transform.position - targetPos);
    }
    private IEnumerator AttackCoroutine()
    {
        StopCoroutine("Chase");
        StopCoroutine("FollowPath");
        float attackDelay = 5f;
        float time = attackDelay;
        isChase = false;
        while (true)
        {

            time += Time.deltaTime;
            if (time >= attackDelay)
            {

                //state = EnemyState.Attack;
                Turn(transform.position, target.position);
                //Debug.Log("TargetName : " + target.name + "(" + dmg + ")");
                time = 0f;
                //UpdateAnimation(state);

                yield return new WaitForSeconds(1f);
                continue;
            }
            //state = EnemyState.Idle;
            //UpdateAnimation(state);
            float dist = (transform.position - target.position).sqrMagnitude;
            if (dist > attackRange * attackRange)
            {
                break;
            }
            yield return null;
        }
        CheckChase();
    }
    private void OnDestroy()
    {
        UnitSelectionManager.Instance.allUnitsList.Remove(gameObject);
    }

}
