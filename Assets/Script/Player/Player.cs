using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 单例实例
    public static Player Instance { get; private set; }
    
    [Header("玩家属性")]
    public int armCount = 2;
    public int legCount = 2;
    
    // 便捷属性
    public Vector3 Position => transform.position;
    public Vector3 Forward => transform.up;
    public Vector3 Right => transform.right;
    
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
    }
    
    // 状态检查方法（这些有必要，因为涉及游戏逻辑）
    public bool CanShoot() => armCount > 0 || legCount > 0;
    public bool CanMove() => legCount > 0;
    
    // 弹药管理方法（核心游戏逻辑，有必要）
    public void ConsumeArm()
    {
        if (armCount > 0) armCount--;
    }
    
    public void ConsumeLeg()
    {
        if (legCount > 0) legCount--;
    }
    
    public void AddArm(int amount = 1) => armCount += amount;
    public void AddLeg(int amount = 1) => legCount += amount;
    
    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
