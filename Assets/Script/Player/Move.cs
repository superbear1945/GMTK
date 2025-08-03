using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.InputSystem;

public class Move : MonoBehaviour
{
    PlayerInput _playerInput;
    InputAction _moveAction;
    Rigidbody2D _rb2d;

    [Header("移动设置")]
    [Tooltip("角色移动速度")]
    public float speed = 5f;

    [Tooltip("只有一条腿时的移动速度倍率")]
    public float oneLegSpeedMultiplier = 0.5f;

    [Tooltip("没有腿时的移动速度倍率")]
    public float noLegSpeedMultiplier = 0.25f;

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

        switch (Player.Instance.legCount)
        {
            case 2:
                _rb2d.MovePosition(_rb2d.position + moveDirection * Time.fixedDeltaTime * speed);
                break;
            case 1:
                // 如果只有一条腿，移动速度减半
                _rb2d.MovePosition(_rb2d.position + moveDirection * Time.fixedDeltaTime * speed * oneLegSpeedMultiplier);
                break;
            default:
                // 没有腿时移动速度四分之一
                _rb2d.MovePosition(_rb2d.position + moveDirection * Time.fixedDeltaTime * speed * noLegSpeedMultiplier);
                break;
        }
    }

}
