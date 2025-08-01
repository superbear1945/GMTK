using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            // 如果敌人碰到玩家，结束游戏（暂定）
            GameManager.Instance.GameEnd();
        }
    }
}
