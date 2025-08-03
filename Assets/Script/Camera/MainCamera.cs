using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    static public MainCamera Instance { get; private set; }
    GameObject _virtualCamera;
    void Awake()
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

        _virtualCamera = transform.Find("Virtual Camera").gameObject;
        if (_virtualCamera == null)
            Debug.LogError("MainCamera: No Virtual Camera found in the hierarchy.");
    }

    void Start()
    {
        _virtualCamera.GetComponent<CinemachineVirtualCamera>().Follow = Player.Instance.transform;
    }
}
