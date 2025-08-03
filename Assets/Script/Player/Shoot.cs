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
        bool isShootArm = false;
        bool isLeft = false; //是否从左侧发射肢体

        //判断按下的是左键还是右键，左键发射手，右键发射腿
        if (context.control.name == "leftButton")
        {
            isShootArm = true;
            if (Player.Instance.armCount == 2) //发射左手
            {
                isLeft = true; // 设置为左侧发射
                //Player.Instance.leftArm.SetActive(false); // 隐藏射出的左手
            }
            else if (Player.Instance.armCount == 1) //发射右手
            {
                isLeft = false; // 设置为右侧发射
                //Player.Instance.rightArm.SetActive(false); // 隐藏射出的右手
            }
            else
            {
                Debug.LogError("No arms available to shoot!");
                return; // 如果没有手臂可用，则不执行射击
            }
        }
        else if (context.control.name == "rightButton")
        {
            isShootArm = false;
            if(Player.Instance.legCount == 2) //发射左脚
            {
                isLeft = true; // 设置为左侧发射
            }
            else if (Player.Instance.legCount == 1) //发射右脚
            {
                isLeft = false; // 设置为右侧发射
            }
            else
            {
                Debug.LogError("No legs available to shoot!");
                return; // 如果没有腿部可用，则不执行射击
            }
        }
        else
            Debug.LogError("Unknown button pressed: " + context.control.name);

        //根据isLeft判断从左侧还是右侧发射肢体
        Vector2 startPosition = GameManager.CurrentPlayer.transform.position; //肢体发射位置，默认为玩家位置
        if(isLeft)
            startPosition = Player.Instance.leftArm.transform.position; //从左侧发射
        else
            startPosition = Player.Instance.rightArm.transform.position; //从右侧发射

        //发射方向为鼠标与身体左侧或者右侧连线
        Vector2 direction = (GameManager.Instance.mousePosition - startPosition).normalized; // 计算发射方向
        Vector2 instancePosition = startPosition + direction * _instanceOffset; // 在左右侧手的位置前生成
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, direction); // 获得玩家目前朝向的旋转

        if (isShootArm == true && Player.Instance.armCount > 0)
        {
            Player.Instance.armCount--; //减少手臂数量
            var armInstance = Instantiate(arm, instancePosition, rotation);
            OnShootEvent?.Invoke(); // 触发射击事件
        }
        else if (isShootArm == false && Player.Instance.legCount > 0)
        {
            Player.Instance.legCount--; //减少腿部数量
            var legInstance = Instantiate(leg, instancePosition, rotation);
            OnShootEvent?.Invoke(); // 触发射击事件
        }
    }

    
}
