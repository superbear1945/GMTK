using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class Destroyable_Door : MonoBehaviour, Interface_Door
{
    [SerializeField] private BoxCollider2D _detectBox;
    [SerializeField] private BoxCollider2D _collidBox;

    public void Start()
    {
        // Ensure there is no ambiguity by using 'this' keyword
        _detectBox.isTrigger = true;
    }
    public void BeginOverlap(GameObject obj)
    {
        if (obj.tag == "Bullet")
        {
            Debug.Log("门检测到了手臂，碰撞关闭");
            _collidBox.enabled = false;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        BeginOverlap(collision.gameObject);
    }


    public void Update()
    {

    }
}


