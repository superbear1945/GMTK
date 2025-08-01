using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Move : MonoBehaviour
{
    PlayerInput _playerInput;
    InputAction _moveAction;
    Rigidbody2D _rb2d;
    public float speed = 5f;

    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _moveAction = _playerInput.actions["Move"];
        if (_moveAction == null)
            Debug.LogError("No Move Action On" + gameObject.name);
        if (_playerInput == null)
            Debug.LogError("No PlayerInput Component On" + gameObject.name);

        _rb2d = GetComponent<Rigidbody2D>();
        if (_rb2d == null)
            Debug.LogError("No Rigidbody2D Component On" + gameObject.name);
    }


    void FixedUpdate()
    {
        PlayerMove();
    }

    void PlayerMove()
    {
        Vector2 moveDirection = _moveAction.ReadValue<Vector2>();
        if(moveDirection != Vector2.zero)
        {
            Player.Instance.isMove = true; // 设置玩家移动状态
        }
        else
        {
            Player.Instance.isMove = false; // 停止移动时重置状态
        }
        _rb2d.MovePosition(_rb2d.position + moveDirection * Time.fixedDeltaTime * speed);
    }

}
