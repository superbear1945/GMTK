using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shoot : MonoBehaviour
{
    private PlayerInput _playerInput;
    private InputAction _shootAction;
    public GameObject arm;
    public GameObject leg;

    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _shootAction = _playerInput.actions["Shoot"];
        if (_playerInput == null)
            Debug.LogError("No PlayerInput Component On " + gameObject.name);
        if (_shootAction == null)
            Debug.LogError("No Shoot Action On " + gameObject.name);

        // 注册射击事件
        _shootAction.performed += OnShoot;
    }

    void OnDisable()
    {
        _shootAction.performed -= OnShoot;  // 取消注册事件，避免内存泄漏
    }

    void OnShoot(InputAction.CallbackContext context)
    {
        
    }
}
