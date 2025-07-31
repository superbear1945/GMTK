using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletBase : MonoBehaviour
{
    public float speed = 5f; // 子弹速度
    public float lifetime = 1f; // 子弹生命周期
    protected Rigidbody2D _rb2d;
    protected bool _isLand = false; // 身体组件是否已经落地
    public System.Action OnHitEvent; // 子弹击中事件

    void Awake()
    {
        _isLand = false; // 初始化为未落地状态
        _rb2d = GetComponent<Rigidbody2D>();
        if (_rb2d == null)
            Debug.LogError("No Rigidbody2D Component On " + gameObject.name);
    }

    void Start()
    {
        Shoot();
    }

    // 使用 virtual 允许子类重写射击行为
    public virtual void Shoot()
    {
        _rb2d.velocity = transform.up * speed;

        //在lifetime后还未击中目标时，则落地等待被拾取
        StartCoroutine(Land());
    }

    IEnumerator Land()
    {
        yield return new WaitForSeconds(lifetime);
        if (!_isLand)
        {
            _isLand = true;
            _rb2d.velocity = Vector2.zero; // 停止移动
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //如果击中敌人或者障碍物
        if (collision.CompareTag("Enemy") || collision.CompareTag("Obstacle"))
        {
            OnHit(collision); //调用子类实现的OnHit方法，实现多态功能
            OnHitEvent?.Invoke(); // 调用击中事件   
        }
    }

    // abstract 方法：子类必须实现OnHit方法
    public abstract void OnHit(Collider2D collision);
}
