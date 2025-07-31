using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shoot : MonoBehaviour
{
    private PlayerInput _playerInput;
    private InputAction _shootAction;
    
    [Header("射击肢体的Prefab")]
    public GameObject arm;
    public GameObject leg;
    
    [SerializeField, Tooltip("子弹生成与玩家的距离"), Header("射击设置")]
    float _instanceOffset = 0.5f; // 实例化偏移量

    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _shootAction = _playerInput.actions["ShootArm"];
        if (_playerInput == null)
            Debug.LogError("No PlayerInput Component On " + gameObject.name);
        if (_shootAction == null)
            Debug.LogError("No Shoot Action On " + gameObject.name);

        // 注册射击事件
        _shootAction.performed += OnShootArm;
    }

    void OnDisable()
    {
        _shootAction.performed -= OnShootArm;  // 取消注册事件，避免内存泄漏
    }

    void OnShootArm(InputAction.CallbackContext context)
    {
        bool isLeft = false;
        if (context.control.name == "leftButton")
            isLeft = true;
        else if (context.control.name == "rightButton")
            isLeft = false;
        Debug.Log(isLeft);
        //获取玩家当前朝向
        Transform playerTransform = GameManager.CurrentPlayer.transform;
        Vector2 direction = playerTransform.right.normalized;
        Vector2 instancePosition = playerTransform.position + (Vector3)direction * _instanceOffset; // 在玩家前方生成

        var armInstance = Instantiate(arm, instancePosition, Quaternion.identity);
        armInstance.transform.up = direction; // 设置朝向

    }
    
}
