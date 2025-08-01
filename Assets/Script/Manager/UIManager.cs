using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI armCountText;
    public TextMeshProUGUI legCountText;

    static public UIManager Instance { get; private set; }

    void Start()
    {
        Player.Instance.GetComponent<Shoot>().OnShootEvent += UpdateCountUI;
    }

    void OnDisable()
    {
        if(Player.Instance != null)
            Player.Instance.GetComponent<Shoot>().OnShootEvent -= UpdateCountUI; // 取消注册事件，避免内存泄漏
    }

    public void UpdateCountUI()
    {
        //检测空引用
        if (armCountText == null || legCountText == null)
        {
            Debug.LogError("No TextMeshPro On " + gameObject.name);
            Debug.LogError("请把表示剩余肢体数量的TextMeshPro组件拖到UIManager的armCountText和legCountText字段上");
            return;
        }

        // 更新UI显示
        armCountText.text = $"X{Player.Instance.armCount}";
        legCountText.text = $"X{Player.Instance.legCount}";
    }

    //单例模式实现
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning($"已存在UIManager实例，销毁重复的对象: {gameObject.name}");
            Destroy(gameObject);
        }
    }
}
