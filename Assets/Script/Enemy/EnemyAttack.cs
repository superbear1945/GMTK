using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("攻击设置")]
    [Tooltip("攻击距离")]
    public float attackDistance = 2f;
    [Tooltip("攻击冷却时间")]
    public float attackCooldown = 1f;
    [Tooltip("攻击持续时间")]
    public float attackDuration = 0.5f;
    [Tooltip("攻击伤害")]
    public int attackDamage = 1;
    
    [Header("攻击检测")]
    [Tooltip("攻击检测的盒子大小")]
    public Vector2 attackBoxSize = new Vector2(1.5f, 1f);
    [Tooltip("攻击检测前方偏移距离（基于transform.up方向）")]
    public float forwardOffset = 0.5f;
    [Tooltip("攻击检测右侧偏移距离（基于transform.right方向）")]
    public float rightOffset = 0f;
    
    [Header("调试信息")]
    [SerializeField, Tooltip("是否开启Gizmos绘制")]
    private bool enableGizmos = true;

    [SerializeField, Tooltip("是否正在攻击中")]
    private bool isAttacking = false;
    [SerializeField, Tooltip("攻击冷却计时器")]
    private float attackCooldownTimer = 0f;

    // 事件系统
    public System.Action<int> OnDamageDealt; // 造成伤害时的事件
    public System.Action OnAttackStart;      // 攻击开始事件
    public System.Action OnAttackEnd;        // 攻击结束事件

    // 私有引用
    private Transform playerTransform;

    void Awake()
    {
        // 缓存玩家引用
        if (Player.Instance != null)
        {
            playerTransform = Player.Instance.transform;
        }
    }

    void Update()
    {
        // 更新攻击冷却计时器
        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        // 更新玩家引用（防止场景重载后引用丢失）
        if (playerTransform == null && Player.Instance != null)
        {
            playerTransform = Player.Instance.transform;
        }
    }

    /// <summary>
    /// 检查是否在攻击范围内
    /// </summary>
    public bool IsInAttackRange()
    {
        if (playerTransform == null) return false;
        
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        return distance <= attackDistance;
    }

    /// <summary>
    /// 检查是否可以攻击（冷却完成且不在攻击中）
    /// </summary>
    public bool CanAttack()
    {
        return !isAttacking && attackCooldownTimer <= 0;
    }

    /// <summary>
    /// 执行攻击
    /// </summary>
    public void StartAttack()
    {
        if (!CanAttack()) return;
        
        StartCoroutine(AttackCoroutine());
    }

    /// <summary>
    /// 强制停止攻击
    /// </summary>
    public void StopAttack()
    {
        if (isAttacking)
        {
            StopCoroutine(AttackCoroutine());
            isAttacking = false;
            OnAttackEnd?.Invoke();
        }
    }

    /// <summary>
    /// 面向目标（通常是玩家）
    /// </summary>
    public void FaceTarget()
    {
        if (playerTransform == null) return;
        
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        transform.up = directionToPlayer; // 2D游戏中通常使用up作为前方
    }

    /// <summary>
    /// 计算攻击检测的中心点（基于敌人的朝向）
    /// </summary>
    private Vector3 GetAttackCenter()
    {
        // 使用敌人的朝向计算偏移量
        Vector3 forwardOffsetVector = transform.up * forwardOffset;      // 前方偏移
        Vector3 rightOffsetVector = transform.right * rightOffset;       // 右侧偏移
        
        return transform.position + forwardOffsetVector + rightOffsetVector;
    }

    /// <summary>
    /// 攻击协程
    /// </summary>
    private IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        OnAttackStart?.Invoke();
        
        Debug.Log($"{gameObject.name} 开始攻击玩家！");
        
        // 攻击预备阶段（可以播放攻击动画的前摇）
        yield return new WaitForSeconds(attackDuration * 0.3f);
        
        // 执行攻击检测
        PerformAttackDetection();
        
        // 攻击后摇阶段
        yield return new WaitForSeconds(attackDuration * 0.7f);
        
        // 设置攻击冷却
        attackCooldownTimer = attackCooldown;
        isAttacking = false;
        
        OnAttackEnd?.Invoke();
        Debug.Log($"{gameObject.name} 攻击完成，冷却时间: {attackCooldown}秒");
    }

    /// <summary>
    /// 执行攻击检测
    /// </summary>
    private void PerformAttackDetection()
    {
        // 计算攻击检测区域的中心点（基于敌人朝向）
        Vector3 attackCenter = GetAttackCenter();
        
        // 使用BoxCast进行攻击检测
        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            attackCenter,
            attackBoxSize,
            transform.eulerAngles.z, // 旋转角度与敌人保持一致
            Vector2.zero,            // 不需要投射方向，因为是原地检测
            0f                       // 距离为0，只检测当前位置
        );

        bool hitPlayer = false;
        
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Player"))
            {
                hitPlayer = true;
                DealDamageToPlayer();
                break;
            }
        }

        if (!hitPlayer)
        {
            Debug.Log($"{gameObject.name} 攻击未命中玩家");
        }
    }

    /// <summary>
    /// 对玩家造成伤害
    /// </summary>
    private void DealDamageToPlayer()
    {
        Debug.Log($"{gameObject.name} 对玩家造成了 {attackDamage} 点伤害！");
        
        // 触发伤害事件
        OnDamageDealt?.Invoke(attackDamage);
        
        // 如果玩家有生命值系统，可以在这里调用
        // Player.Instance.TakeDamage(attackDamage);
        
        // 或者直接结束游戏（如果是一击必杀的游戏）
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameEnd();
        }
    }

    /// <summary>
    /// 获取攻击状态信息
    /// </summary>
    public bool IsAttacking => isAttacking;
    public float AttackCooldownRemaining => attackCooldownTimer;
    public float AttackDistance => attackDistance;

    /// <summary>
    /// 在Scene视图中绘制攻击范围
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if(!enableGizmos) return; //不开启Gizmos绘制
        
        // 绘制攻击距离范围
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
        
        // 绘制攻击检测区域
        Gizmos.color = Color.cyan;
        Vector3 attackCenter = GetAttackCenter();
        
        // 绘制攻击盒子（考虑敌人的旋转）
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(attackCenter, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, attackBoxSize);
        Gizmos.matrix = oldMatrix;
        
        // 绘制朝向指示器
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.up * 1f); // 前方方向
        
        // 绘制偏移中心点
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackCenter, 0.1f);
    }
}
