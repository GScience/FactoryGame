using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 玩家输入管理
/// </summary>
public class PlayerInput : MonoBehaviour
{
    public static InstanceHelper<PlayerInput> GlobalInput;

    private readonly Vector2?[] _mouseDownPos = new Vector2?[2];
    private Camera _camera;

    private Vector2 viewportPos;

    void Awake()
    {
        GlobalInput = new InstanceHelper<PlayerInput>(this);
        _camera = Camera.main;
    }

    void LateUpdate()
    {
        for (var i = 0; i < _mouseDownPos.Length; ++i)
        {
            if (_mouseDownPos[i] != Input.mousePosition)
                _mouseDownPos[i] = null;

            if (Input.GetMouseButtonDown(i))
                _mouseDownPos[i] = Input.mousePosition;
        }

        var mousePos = (Vector2)Input.mousePosition;
        var worldPos = _camera.ScreenToWorldPoint(mousePos);
        viewportPos = new Vector3(worldPos.x, worldPos.y, 1);
    }

    public static bool GetMouseClick(int button)
    {
        return Input.GetMouseButtonUp(button) && GlobalInput.Get()._mouseDownPos[button] != null;
    }

    public static Vector2 GetMousePosInWorld()
    {
        return GlobalInput.Get().viewportPos;
    }

    /// <summary>
    /// 是否正在建造
    /// </summary>
    /// <returns></returns>
    public static bool IsBuilding()
    {
        return 
            BeltBuilder.GlobalBuilder.Get().IsBuilding || 
            BuildingBuilder.GlobalBuilder.Get().IsBuilding;
    }
}
