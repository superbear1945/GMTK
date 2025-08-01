using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using EnemyAi;

public class FindPlayer : MonoBehaviour
{
    AIPath _aiPath;
    AIDestinationSetter _aiDestinationSetter;
    [Header("搜索玩家位置相关设置")]
    [Tooltip("搜索半径")]
    public float findRadius = 5f;
    [Tooltip("是否绘制搜索范围Gizmos")]
    public bool drawGizmos = false; // 是否绘制Gizmos

    private void Awake()
    {
        _aiPath = GetComponent<AIPath>();
        if (_aiPath == null)
            Debug.LogError("No AIPath Component On " + gameObject.name);

        _aiDestinationSetter = GetComponent<AIDestinationSetter>();
        if (_aiDestinationSetter == null)
            Debug.LogError("No AIDestinationSetter Component On " + gameObject.name);
    }

    void Start()
    {
        _aiDestinationSetter.target = Player.Instance?.transform; // 设置目标为玩家
        if (_aiDestinationSetter.target == null)
        {
            Debug.LogWarning("Player实例未找到，无法设置目标");
            return;
        }
    }

    void Update()
    {
        _aiPath.canMove = Find(); // 根据是否找到玩家来决定是否移动
    }


    //判断玩家是否处于搜索半径内，且是否中间有阻挡
    bool Find()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, Player.Instance.transform.position);
        if (distanceToPlayer <= findRadius)
        {
            Vector2 direction = (Player.Instance.transform.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distanceToPlayer, LayerMask.GetMask("Obstacle"));
            if (hit.collider != null)
            {
                return false;
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnDrawGizmos()
    {
        if(!drawGizmos) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, findRadius); // 绘制寻找范围
    }
}
