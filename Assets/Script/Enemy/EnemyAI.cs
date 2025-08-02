using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public enum EnemyState
{
    Idle,       // 闲置状态
    Patrol,     // 巡逻状态
    Chase,      // 追逐状态
    Attack      // 攻击状态
}


public class EnemyAI : MonoBehaviour
{
    [Header("AI状态")]
    public EnemyState currentState;

    [Header("AI组件")]
    AIPath _aiPath;
    AIDestinationSetter _destinationSetter;
    EnemyAttack _enemyAttack;

    [Header("AI属性")]
    [Tooltip("是否找到玩家")]
    public bool isFindPlayer = false;

    [Header("巡逻设置")]
    [Tooltip("巡逻半径")]
    public float patrolRadius = 5f;
    [Tooltip("到达巡逻点后的等待时间")]
    public float patrolWaitTime = 2f;
    [Tooltip("闲置状态持续时间（之后开始巡逻）")]
    public float idleTime = 1f;
    [Tooltip("到达巡逻点的检测距离")]
    public float reachDistance = 1f;

    [Header("调试信息")]
    [SerializeField, Tooltip("当前巡逻目标点")]
    private Vector3 currentPatrolTarget;
    [SerializeField, Tooltip("是否正在巡逻中")]
    private bool isPatrolling = false;
    [SerializeField, Tooltip("是否正在闲置中")]
    private bool isIdling = false;

    // 私有变量
    private GameObject currentPatrolTargetObject;
    private Vector3 initialPosition; // 记录初始位置作为巡逻中心点

    void Awake()
    {
        _aiPath = GetComponent<AIPath>();
        _destinationSetter = GetComponent<AIDestinationSetter>();
        _enemyAttack = GetComponent<EnemyAttack>();

        if (_aiPath == null)
            Debug.LogError("No AIPath Component On " + gameObject.name);
        if (_destinationSetter == null)
            Debug.LogError("No AIDestinationSetter Component On " + gameObject.name);
        if (_enemyAttack == null)
            Debug.LogError("No EnemyAttack Component On " + gameObject.name);
    }

    void Start()
    {
        currentState = EnemyState.Idle;
        initialPosition = transform.position;

        // 订阅攻击事件
        if (_enemyAttack != null)
        {
            _enemyAttack.OnAttackStart += OnAttackStarted;
            _enemyAttack.OnAttackEnd += OnAttackEnded;
            _enemyAttack.OnPlayerHit += OnPlayerHit; // 修改：订阅击中玩家事件
        }
    }

    void OnDestroy()
    {
        // 取消攻击事件订阅
        if (_enemyAttack != null)
        {
            _enemyAttack.OnAttackStart -= OnAttackStarted;
            _enemyAttack.OnAttackEnd -= OnAttackEnded;
            _enemyAttack.OnPlayerHit -= OnPlayerHit; // 修改：取消订阅击中玩家事件
        }

        // 清理巡逻目标对象
        if (currentPatrolTargetObject != null)
        {
            Destroy(currentPatrolTargetObject);
        }
    }

    void Update()
    {
        SwitchState();
    }

    void SwitchState()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                OnIdle();
                break;
            case EnemyState.Patrol:
                OnPatrol();
                break;
            case EnemyState.Chase:
                OnChase();
                break;
            case EnemyState.Attack:
                OnAttack();
                break;
            default:
                Debug.LogError("未知的敌人状态: " + currentState);
                break;
        }
    }

    private void OnAttack()
    {
        if (!isFindPlayer || Player.Instance == null)
        {
            // 如果失去玩家目标，返回闲置状态
            _enemyAttack.StopAttack();
            currentState = EnemyState.Idle;
            return;
        }

        // 如果玩家离得太远，切换回追逐状态
        // if (!_enemyAttack.IsInAttackRange())
        // {
        //     _enemyAttack.StopAttack();
        //     currentState = EnemyState.Chase;
        //     return;
        // }

        // 停止移动，准备攻击
        _aiPath.canMove = false;
        _destinationSetter.target = null;

        // 面向玩家
        _enemyAttack.FaceTarget();

        // 执行攻击
        if (_enemyAttack.CanAttack())
        {
            _enemyAttack.StartAttack();
        }
    }

    // 攻击事件回调
    private void OnAttackStarted()
    {
        Debug.Log($"{gameObject.name} AI: 攻击开始");
    }

    private void OnAttackEnded()
    {
        Debug.Log($"{gameObject.name} AI: 攻击结束");
    }

    private void OnPlayerHit() // 修改：新的击中玩家回调
    {
        Debug.Log($"{gameObject.name} AI: 击中玩家！");
    }

    private void OnIdle()
    {
        if (isFindPlayer)
        {
            currentState = EnemyState.Chase;
            return;
        }

        _aiPath.canMove = false;
        _destinationSetter.target = null;

        // 闲置一段时间后开始巡逻
        if (!isIdling)
        {
            StartCoroutine(IdleTimer());
        }
    }

    private IEnumerator IdleTimer()
    {
        isIdling = true;
        yield return new WaitForSeconds(idleTime);

        if (currentState == EnemyState.Idle && !isFindPlayer)
        {
            currentState = EnemyState.Patrol;
        }

        isIdling = false;
    }

    private void OnPatrol()
    {
        if (isFindPlayer)
        {
            StopPatrol();
            currentState = EnemyState.Chase;
            return;
        }

        _aiPath.canMove = true;

        // 如果还没有开始巡逻，开始巡逻协程
        if (!isPatrolling)
        {
            StartCoroutine(PatrolCoroutine());
        }
    }

    private IEnumerator PatrolCoroutine()
    {
        isPatrolling = true;

        while (currentState == EnemyState.Patrol && !isFindPlayer)
        {
            // 寻找有效的巡逻点
            Vector3 validPatrolPoint = GetValidPatrolPoint();

            if (validPatrolPoint != Vector3.zero)
            {
                // 设置巡逻目标
                SetPatrolTarget(validPatrolPoint);

                // 等待到达目标点
                yield return StartCoroutine(WaitForReachTarget(validPatrolPoint));

                // 到达后等待一段时间
                yield return new WaitForSeconds(patrolWaitTime);
            }
            else
            {
                Debug.LogWarning($"敌人 {gameObject.name} 无法找到有效巡逻点，等待重试...");
                yield return new WaitForSeconds(patrolWaitTime);
            }
        }

        isPatrolling = false;
    }

    private Vector3 GetValidPatrolPoint()
    {
        int maxAttempts = 10;

        for (int i = 0; i < maxAttempts; i++)
        {
            // 在圆形范围内生成随机点（以初始位置为中心）
            Vector2 randomDirection = UnityEngine.Random.insideUnitCircle * patrolRadius;
            Vector3 targetPoint = initialPosition + new Vector3(randomDirection.x, randomDirection.y, 0);

            // 检查该点是否可达
            if (IsValidPatrolPoint(targetPoint))
            {
                return targetPoint;
            }
        }

        return Vector3.zero; // 表示没有找到有效点
    }

    private bool IsValidPatrolPoint(Vector3 point)
    {
        // 检查A*寻路是否可达该点
        if (AstarPath.active == null) return true; // 如果没有A*图，假设可达

        NNInfo nearestInfo = AstarPath.active.GetNearest(point);
        if (nearestInfo.node == null || !nearestInfo.node.Walkable)
        {
            return false;
        }

        // 检查距离是否合理
        if (Vector3.Distance(nearestInfo.position, point) > 2f)
        {
            return false;
        }

        // 检查从当前位置是否可达
        NNInfo startInfo = AstarPath.active.GetNearest(transform.position);
        if (startInfo.node == null || !startInfo.node.Walkable)
        {
            return false;
        }

        return PathUtilities.IsPathPossible(startInfo.node, nearestInfo.node);
    }

    private void SetPatrolTarget(Vector3 targetPosition)
    {
        currentPatrolTarget = targetPosition;

        // 清理之前的目标对象
        if (currentPatrolTargetObject != null)
        {
            Destroy(currentPatrolTargetObject);
        }

        // 创建新的巡逻目标
        currentPatrolTargetObject = new GameObject($"PatrolTarget_{gameObject.name}");
        currentPatrolTargetObject.transform.position = targetPosition;
        _destinationSetter.target = currentPatrolTargetObject.transform;
    }

    private IEnumerator WaitForReachTarget(Vector3 targetPosition)
    {
        float timeout = 10f; // 超时时间，避免AI卡住
        float elapsed = 0f;

        while (elapsed < timeout && currentState == EnemyState.Patrol && !isFindPlayer)
        {
            float distance = Vector3.Distance(transform.position, targetPosition);

            if (distance <= reachDistance)
            {
                break; // 到达目标点
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void OnChase()
    {
        if (!isFindPlayer)
        {
            StopChase();
            currentState = EnemyState.Idle;
            return;
        }

        if (Player.Instance == null)
        {
            Debug.LogWarning("Player.Instance 为空，无法追逐");
            currentState = EnemyState.Idle;
            return;
        }

        // 检查是否进入攻击范围
        if (_enemyAttack.IsInAttackRange())
        {
            // 进入攻击状态
            currentState = EnemyState.Attack;
            return;
        }

        _aiPath.canMove = true;
        _destinationSetter.target = Player.Instance.transform;
    }

    private void StopPatrol()
    {
        if (currentPatrolTargetObject != null)
        {
            Destroy(currentPatrolTargetObject);
            currentPatrolTargetObject = null;
        }

        StopAllCoroutines();
        isPatrolling = false;
        isIdling = false;
    }

    private void StopChase()
    {
        _destinationSetter.target = null;
    }

    // 公共方法：外部调用来设置是否发现玩家
    public void SetPlayerFound(bool found)
    {
        isFindPlayer = found;
    }

    
}
