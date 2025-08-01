using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyAi;

namespace EnemyAi
{
    enum EnemyState
    {
        Idle,
        Patrol,
        Chase
    }
}

public class EnemyAI : MonoBehaviour
    {
        EnemyState currentState;

        void Start()
        {
            currentState = EnemyState.Idle; // 初始状态为闲置
        }

        void Update()
        {
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

        }

        private void OnPatrol()
        {

        }

        private void OnIdle()
        {

        }
    }
