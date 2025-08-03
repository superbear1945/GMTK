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

        // 如果 Inspector 中没有手动赋值，则自动查找
        if (armCountText == null)
        {
            // GameObject.Find 效率较低，但对于初始化来说是可接受的
            Transform textTransform = GameObject.Find("ArmCountTMP")?.transform;
            if (textTransform != null)
                armCountText = textTransform.GetComponent<TextMeshProUGUI>();
        }

        if (legCountText == null)
        {
            Transform textTransform = GameObject.Find("LegCountTMP")?.transform;
            if (textTransform != null)
                legCountText = textTransform.GetComponent<TextMeshProUGUI>();
        }
    }

    void OnDisable()
    {
        if(Player.Instance != null)
            Player.Instance.GetComponent<Shoot>().OnShootEvent -= UpdateCountUI; // 取消注册事件，避免内存泄漏
    }

    //更新肢体数量UI
    public void UpdateCountUI()
    {
        //检测空引用
        if (armCountText == null || legCountText == null)
        {
            Debug.LogError("请把MainCanvas中表示剩余肢体数量的TextMeshPro组件拖到UIManager的armCountText和legCountText字段上");
            return;
        }

        // 更新UI显示
        armCountText.text = $"X{Player.Instance.armCount}";
        legCountText.text = $"X{Player.Instance.legCount}";
        Debug.Log(armCountText.text);
    }

    //单例模式实现
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 保持UIManager在场景切换时不被销毁
        }
        else
        {
            Debug.LogWarning($"已存在UIManager实例，销毁重复的对象: {gameObject.name}");
            Destroy(gameObject);
        }
    }
}
