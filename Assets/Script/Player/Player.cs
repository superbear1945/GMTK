using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 单例实例
    public static Player Instance { get; private set; }

    //利用手脚数量确定左右手，默认先发射左手和左脚
    //当Count为2时，发射和判定都是左边，Count为1则为右边
    [Header("玩家属性")]
    public int armCount = 2;
    public int legCount = 2;
    public Transform leftArmTransform;
    public Transform rightArmTransform;

    // 便捷属性
    public Vector3 Position => transform.position;
    public Vector3 Forward => transform.up;
    public Vector3 Right => transform.right;
    public bool isMove = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning($"已存在Player实例，销毁重复的对象: {gameObject.name}");
            Destroy(gameObject);
        }
        
        if(leftArmTransform == null || rightArmTransform == null)
        {
            Debug.LogError("请在Player组件中设置左手和右手的Transform引用");
        }
    }
    
    // 状态检查方法（这些有必要，因为涉及游戏逻辑）
    public bool CanShoot() => armCount > 0 || legCount > 0;
    public bool CanMove() => legCount > 0;
    
    // 弹药管理方法，包括肢体的使用和回收
    public void ConsumeArm() { if (armCount > 0) armCount--; }
    
    public void ConsumeLeg() { if (legCount > 0) legCount--; }

    public void AddArm() { if (armCount < 2) armCount++; }
    public void AddLeg() { if (legCount < 2) legCount++; }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
