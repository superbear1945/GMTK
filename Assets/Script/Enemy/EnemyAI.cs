using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyAi;
using Pathfinding;

namespace EnemyAi
{
    public enum EnemyState
    {
        Idle,
        Patrol,
        Chase
    }
}

public class EnemyAI : MonoBehaviour
{
    public EnemyState currentState;
    AIPath _aiPath;
    AIDestinationSetter _destinationSetter;

    [Header("Ai属性")]
    [Tooltip("是否找到玩家")]
    public bool isFindPlayer = false;
    [Tooltip("巡逻半径")]
    public float patrolRadius = 5f;
    [Tooltip("巡逻等待时间")]
    public float patrolWaitTime = 2f;

    void Awake()
    {
        _aiPath = GetComponent<AIPath>();
        _destinationSetter = GetComponent<AIDestinationSetter>();

        if (_aiPath == null)
            Debug.LogError("No AIPath Component On " + gameObject.name);
        if (_destinationSetter == null)
            Debug.LogError("No AIDestinationSetter Component On " + gameObject.name);
    }

    void Start()
    {
        currentState = EnemyState.Idle; // 初始状态为闲置
    }
    
    void Update()
    {
        SwitchState(); // 根据当前状态执行相应逻辑
        
    }

    void SwitchState()
    {
        //状态切换
        switch (currentState)
        {
            case EnemyState.Idle:
                {
                    // 执行闲置状态逻辑
                    OnIdle();
                    break;
                }
            case EnemyState.Patrol:
                {
                    // 执行巡逻状态逻辑
                    OnPatrol();
                    break;
                }
            case EnemyState.Chase:
                {
                    // 执行追逐状态逻辑
                    OnChase();
                    break;
                }
            default:
                Debug.LogError("未知的敌人状态: " + currentState);
                break;
        }
    }

    private void OnChase()
    {
        if(!isFindPlayer)
        {
            currentState = EnemyState.Idle; // 如果没有找到玩家，切换到闲置状态
            return;
        }

        _aiPath.canMove = true; // 允许移动
        _destinationSetter.target = Player.Instance.transform; // 设置目标为玩家
    }

    private void OnPatrol()
    {
        if(isFindPlayer)
        {
            currentState = EnemyState.Chase; // 如果找到玩家，切换到追逐状态
            return;
        }

        _aiPath.canMove = true; // 允许移动
        StartCoroutine(Patrol()); // 开始巡逻
        
    }

    // 巡逻逻辑
    private IEnumerator Patrol()
    {
        
        //生成随机巡逻点
        Vector3 randomPoint = transform.position + new Vector3(
            UnityEngine.Random.Range(-patrolRadius, patrolRadius),
            UnityEngine.Random.Range(-patrolRadius, patrolRadius),
            0
        );
        // 创建一个临时的GameObject作为巡逻目标点
        GameObject patrolTarget = new GameObject("PatrolTarget");
        patrolTarget.transform.position = randomPoint;
        _destinationSetter.target = patrolTarget.transform;
        yield return new WaitForSeconds(patrolWaitTime); // 等待一段时间
    }

    private void OnIdle()
    {
        if(isFindPlayer)
        {
            currentState = EnemyState.Chase; // 如果找到玩家，切换到追逐状态
            return;
        }

        _aiPath.canMove = false; // 停止移动
        _destinationSetter.target = null; // 清除目标
    }
}
