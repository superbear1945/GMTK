using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 确保引入SceneManagement命名空间

public class GameManager : MonoBehaviour
{
    static public GameManager Instance { get; private set; }
    public Transform spawnPoint;

    /// 当前玩家在GameObject的全局访问点（向后兼容）
    static public GameObject CurrentPlayer
    {
        get
        {
            return Player.Instance != null ? Player.Instance.gameObject : null;
        }
    }

    public void GameEnd()
    {
        SceneManager.LoadScene("SampleScene");
        CurrentPlayer.transform.position = spawnPoint.position; // 重置玩家位置到生成点
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

        if (spawnPoint == null)
            Debug.LogError("Spawn Point未设置，请在GameManager中设置出生点");
        
        CurrentPlayer.transform.position = spawnPoint.position; // 确保玩家在游戏开始时位于生成点
    }
}
