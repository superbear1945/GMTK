using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletBase : MonoBehaviour
{
    public float speed = 5f; // 子弹速度
    public float lifetime = 1f; // 子弹生命周期
    protected Rigidbody2D _rb2d;
    protected bool _isLand = false; // 身体组件是否已经落地
    static public event System.Action<Collider2D> OnHitEvent;

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

    void OnCollisionEnter2D(Collision2D collision)
    {
        //如果击中敌人或者障碍物
        if (collision.collider.CompareTag("Enemy") || collision.collider.CompareTag("Obstacle"))
        {
            OnHitEvent?.Invoke(collision.collider);
            OnHit(collision.collider); //调用子类实现的OnHit方法，实现多态功能
        }

        if (_isLand && collision.collider.CompareTag("Player"))
        {
            OnPickUp(); // 调用子类实现的OnPickUp方法，实现多态功能
            Destroy(gameObject); // 被拾取后销毁地上的肢体
        }
    }

    // abstract 方法：子类必须实现OnHit方法
    public abstract void OnHit(Collider2D collision);

    //子类必须实现被捡起的方法
    public abstract void OnPickUp();
}
