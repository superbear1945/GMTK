using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable_Door : MonoBehaviour,Interface_Door
{
    [SerializeField] private BoxCollider2D _detectBox;
    [SerializeField] private BoxCollider2D _collidBox;


    public void Start()
    {
        _detectBox.isTrigger = true;
    }


    //碰撞逻辑？？
    private void OnCollisionEnter2D(Collision2D collision)
    {
        BeginOverlap(collision.gameObject);
    }

    public void BeginOverlap(GameObject obj)
    {
        if(obj.tag=="Bullet")//检测碰撞者是否为手臂
        {
            Debug.Log("门检测到了手臂，碰撞关闭");
            _collidBox.enabled = false;
        }
    }

    // Update is called once per frame
    public void Update()
    {

    }

}


