using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Action = System.Action;

public class Shoot : MonoBehaviour
{
    private PlayerInput _playerInput;
    private InputAction _shootAction;

    [Header("射击肢体的Prefab")]
    public GameObject arm;
    public GameObject leg;

    [SerializeField, Tooltip("子弹生成与玩家的距离"), Header("射击设置")]
    float _instanceOffset = 0.5f; // 实例化偏移量

    public event Action OnShootEvent; // 射击事件

    public void AddArm()
    {
        if (Player.Instance.armCount < 2)
            Player.Instance.armCount++;
    }

    public void AddLeg()
    {
        if (Player.Instance.legCount < 2)
            Player.Instance.legCount++;
    }

    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _shootAction = _playerInput.actions["ShootArm"];
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
        bool isLeft = false;
        //判断按下的是左键还是右键
        if (context.control.name == "leftButton")
            isLeft = true;
        else if (context.control.name == "rightButton")
            isLeft = false;
        else
            Debug.LogError("Unknown button pressed: " + context.control.name);

        //获取玩家当前朝向 - 改为使用 up 作为正面
        Transform playerTransform = GameManager.CurrentPlayer.transform;
        Vector2 direction = playerTransform.up.normalized; // 从 right 改为 up
        Vector2 instancePosition = playerTransform.position + (Vector3)direction * _instanceOffset; // 在玩家前方生成
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, direction); // 获得玩家目前朝向的旋转

        if (isLeft == true && Player.Instance.armCount > 0)
        {
            Player.Instance.armCount--; //减少手臂数量
            var armInstance = Instantiate(arm, instancePosition, rotation);
            OnShootEvent?.Invoke(); // 触发射击事件
        }
        else if (isLeft == false && Player.Instance.legCount > 0)
        {
            Player.Instance.legCount--; //减少腿部数量
            var legInstance = Instantiate(leg, instancePosition, rotation);
            OnShootEvent?.Invoke(); // 触发射击事件
        }
    }

    
}
