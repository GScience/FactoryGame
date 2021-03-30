using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 相机控制器
/// 创建一个假的物体表示屏幕中央
/// </summary>
[RequireComponent(typeof(CinemachineVirtualCamera))]
public class VirtualCameraController : MonoBehaviour
{
    private GameObject _viewportCenter;
    private CinemachineVirtualCamera _virtualCamera;

    private Camera _camera;
    private Vector2? _lastMousePos;

    void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _camera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        _viewportCenter = new GameObject("(Viewport Center)");
        _virtualCamera.Follow = _viewportCenter.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        var isMouseOnUI = EventSystem.current.IsPointerOverGameObject();

        // 移动
        if (Input.GetMouseButtonDown(0) && !isMouseOnUI)
            _lastMousePos = Input.mousePosition;
        else if (Input.GetMouseButton(0) && _lastMousePos != null)
        {
            var mousePos = (Vector2)Input.mousePosition;

            var viewportDeltaPos =
                _camera.ScreenToWorldPoint(_lastMousePos.Value) - _camera.ScreenToWorldPoint(mousePos);
            _viewportCenter.transform.position += viewportDeltaPos;

            _lastMousePos = mousePos;
        }
        else
            _lastMousePos = null;

        // 缩放
        if (!GameMap.GlobalMap.Get().isBuilding)
        {
            _virtualCamera.m_Lens.OrthographicSize -= Input.mouseScrollDelta.y;
            if (_virtualCamera.m_Lens.OrthographicSize < 10)
                _virtualCamera.m_Lens.OrthographicSize = 10;
            else if (_virtualCamera.m_Lens.OrthographicSize > 25)
                _virtualCamera.m_Lens.OrthographicSize = 25;
        }
    }

    public void MoveTo(Vector2 pos)
    {
        _viewportCenter.transform.position = pos;
    }

    public Vector2 GetPosition()
    {
        return _viewportCenter.transform.position;
    }
}
