using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static public GameManager Instance { get; private set; }
    
    /// 当前玩家在GameObject的全局访问点（向后兼容）
    static public GameObject CurrentPlayer 
    { 
        get 
        { 
            return Player.Instance != null ? Player.Instance.gameObject : null;
        } 
    }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // 验证 Player 实例是否存在
        if (Player.Instance != null)
        {
            Debug.Log("Player实例已就绪");
        }
        else
        {
            Debug.LogWarning("场景中未找到Player实例！");
        }
    }
}
