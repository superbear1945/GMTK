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
    }

}
