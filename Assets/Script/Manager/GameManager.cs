using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 确保引入SceneManagement命名空间

public class GameManager : MonoBehaviour
{
    static public GameManager Instance { get; private set; }
    public Transform spawnPoint;
    public Vector2 mousePosition; // 鼠标位置

    //处理游戏逻辑的部分
    public bool _getKey { get; private set; } = false;// 是否获取到钥匙
    public void SetHasKey(Interface_Keypriviledge caller)
    {
        _getKey = true;
    }


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
        //spawnPoint = GameObject.FindWithTag("SpawnPoint")?.transform; // 查找名为 SpawnPoint 的 GameObject
        if (spawnPoint == null)
            Debug.LogError("Spawn Point未设置，请在GameManager中设置出生点");

        SceneManager.sceneLoaded += OnSceneLoaded; // 订阅场景加载事件
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 取消订阅场景加载事件
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        spawnPoint = GameObject.FindWithTag("SpawnPoint")?.transform; // 查找名为 SpawnPoint 的 GameObject


        // 确保玩家在新场景加载后仍然位于生成点
        if (CurrentPlayer != null && spawnPoint != null)
        {
            CurrentPlayer.transform.position = spawnPoint.position;
        }
    }
}
