using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Data;

public class AimToMouse : MonoBehaviour
{
    [Header("2D瞄准设置")]
    [SerializeField] private bool useSmoothing = true;
    [SerializeField] private float rotationSpeed = 10f;
    
    private PlayerInput _playerInput;
    private InputAction _mouseAction;
    private Camera _camera;

    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _mouseAction = _playerInput.actions["MousePosition"];
        RefreshCameraReference();
        
        if (_mouseAction == null)
            Debug.LogError("No Mouse Action On " + gameObject.name);
        if (_playerInput == null)
            Debug.LogError("No PlayerInput Component On " + gameObject.name);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 场景加载完成后，重新获取相机引用
        StartCoroutine(RefreshCameraAfterDelay());
    }

    private IEnumerator RefreshCameraAfterDelay()
    {
        // 等待一帧，确保新场景的相机已经初始化
        yield return null;
        RefreshCameraReference();
    }

    private void RefreshCameraReference()
    {
        _camera = Camera.main;
        if (_camera == null)
        {
            _camera = FindObjectOfType<Camera>();
            Debug.LogWarning("Camera.main not found, using FindObjectOfType<Camera>");
        }
        
        if (_camera == null)
            Debug.LogError("No camera found in the scene!");
    }

    void Update()
    {
        AimAtMouse();
    }

    private void AimAtMouse()
    {
        if (_mouseAction == null || _camera == null) return;

        // 获取鼠标屏幕位置
        Vector2 mouseScreenPosition = _mouseAction.ReadValue<Vector2>();
        
        // 将屏幕坐标转换为世界坐标（2D）
        Vector3 mouseWorldPosition = _camera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, _camera.nearClipPlane));
        mouseWorldPosition.z = 0; // 确保Z轴为0

        GameManager.Instance.mousePosition = mouseWorldPosition; // 更新GameManager中的鼠标位置

        // 计算从角色到鼠标的方向（2D平面）
        Vector2 direction = (mouseWorldPosition - transform.position).normalized;
        
        // 检查方向向量是否有效
        if (direction.sqrMagnitude < 0.001f) return;

        
        // 计算从当前 up 方向到目标方向的角度
        float angle = Vector2.SignedAngle(Vector2.up, direction);
        
        // 创建目标旋转
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        // 应用旋转
        if (useSmoothing)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            transform.rotation = targetRotation;
        }
    }
}
