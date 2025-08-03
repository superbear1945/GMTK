using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer_UI : MonoBehaviour
{
    private TMP_Text time;
    private float _totalTime = 60.0f;
    private float _currentTime;
    // Start is called before the first frame update
    void Start()
    {
        time = GetComponent<TMP_Text>();
        _currentTime = _totalTime;
    }

    void Update()
    {
        _currentTime -= Time.deltaTime;

        if (_currentTime <= 0f)
        {
            _currentTime = 0f;
            TimerCompleted();
        }

        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        time.text = $"Time: {_currentTime:F2}"; // 保留两位小数
    }

    void TimerCompleted()
    {
        // 计时器完成时的逻辑
        GameManager.Instance.RestartLevel();
    }

}
