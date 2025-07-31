using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
        _camera = Camera.main;
        
        if (_mouseAction == null)
            Debug.LogError("No Mouse Action On " + gameObject.name);
        if (_playerInput == null)
            Debug.LogError("No PlayerInput Component On " + gameObject.name);
        if (_camera == null)
            Debug.LogError("No Main Camera found for " + gameObject.name);
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
        
        // 计算从角色到鼠标的方向（2D平面）
        Vector3 direction = mouseWorldPosition - transform.position;
        direction.z = 0f; // 确保Z轴为0
        
        // 检查方向向量是否有效（避免除零错误）
        if (direction.sqrMagnitude < 0.001f) return;
        
        // 计算旋转角度
        // Atan2: 计算向量角度，返回弧度值(-π到π)
        // Rad2Deg: 将弧度转换为角度制(×57.2958)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // 创建目标旋转：绕Z轴(forward)旋转angle度
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        // 应用旋转
        if (useSmoothing)
        {
            // Slerp: 球面线性插值，平滑旋转
            // 每帧移动 rotationSpeed * deltaTime 的百分比
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            // 立即设置为目标旋转
            transform.rotation = targetRotation;
        }
    }
}
