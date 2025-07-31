using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static public GameManager Instance { get; private set; }
    
    [SerializeField]private GameObject _currentPlayer;
    
    /// 当前玩家在GameObject的全局访问点
    static public GameObject CurrentPlayer 
    { 
        get 
        { 
            if (Instance == null)
            {
                Debug.LogError("GameManager Instance is null. Make sure GameManager is in the scene.");
                return null;
            }
            
            // 如果_currentPlayer为空，尝试通过Tag查找
            if (Instance._currentPlayer == null)
            {
                Instance._currentPlayer = GameObject.FindGameObjectWithTag("Player");
                if (Instance._currentPlayer == null)
                {
                    Debug.LogError("No GameObject with 'Player' tag found in the scene.");
                }
            }
            
            return Instance._currentPlayer; 
        } 
        set 
        { 
            if (Instance != null)
                Instance._currentPlayer = value; 
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
        // 在游戏开始时主动查找玩家
        if (_currentPlayer == null)
        {
            _currentPlayer = GameObject.FindGameObjectWithTag("Player");
            if (_currentPlayer != null)
                Debug.Log("已有Player实例");
            else
                Debug.LogWarning("场景中未找到带有'Player'标签的GameObject。");
        }
    }
}
