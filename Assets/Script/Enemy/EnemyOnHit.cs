using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyOnHit : MonoBehaviour
{
    void Start()
    {
        BulletBase.OnHitEvent += OnHit; // 订阅子弹击中事件
    }

    void OnDestroy()
    {
        BulletBase.OnHitEvent -= OnHit; // 取消订阅子弹击中事件
    }

    void OnHit(Collider2D collision)
    {
        if (collision.gameObject == gameObject)
        {
            Destroy(gameObject);
            //可以编写其它死亡逻辑
        }
    }
}
