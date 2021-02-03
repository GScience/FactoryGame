using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 工厂建造器
/// 移动、建造建筑时让建筑在鼠标位置
/// </summary>
public class FactoryBuilder : MonoBehaviour
{
    public static InstanceHelper<FactoryBuilder> GlobalBuilder;

    private Building _pickedBuilding;
    private Action _onConfirm;
    private Action _onCancel;

    private Camera _camera;

    private Vector2? _downMousePos;

    public GridRenderer gridRenderer;

    void Awake()
    {
        GlobalBuilder = new InstanceHelper<FactoryBuilder>(this);
        _camera = Camera.main;
    }

    void Start()
    {
        gridRenderer.enabled = false;
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (_pickedBuilding == null)
            return;

        var mousePos = (Vector2) Input.mousePosition;
        var viewportPos = _camera.ScreenToWorldPoint(mousePos);
        _pickedBuilding.transform.position = new Vector3(viewportPos.x, viewportPos.y, 1);

        if (_downMousePos.HasValue && _downMousePos != mousePos)
            _downMousePos = null;
        if (Input.GetMouseButtonDown(0))
            _downMousePos = mousePos;
        if (Input.GetMouseButtonUp(0) && _downMousePos.HasValue)
            OnConfirm();
        else if (Input.GetKey(KeyCode.Escape))
            OnCancel();
    }

    public void Pick(Building obj, Action onConfirm, Action onCancel)
    {
        if (_pickedBuilding != null)
            OnCancel();

        _pickedBuilding = obj;
        _onConfirm = onConfirm;
        _onCancel = onCancel;

        var mousePos = (Vector2)Input.mousePosition;
        var viewportPos = _camera.ScreenToWorldPoint(mousePos);
        _pickedBuilding.transform.position = viewportPos;

        BuildingInformationBoard.GlobalBuildingInformationBoard.Get().ShowInformation(_pickedBuilding);
        gridRenderer.enabled = true;
    }

    void OnConfirm()
    {
        if (_pickedBuilding == null)
            return;
        _onConfirm?.Invoke();
        _pickedBuilding = null;

        BuildingInformationBoard.GlobalBuildingInformationBoard.Get().HideInformation();

        gridRenderer.enabled = false;
    }

    void OnCancel()
    {
        if (_pickedBuilding == null)
            return;
        _onCancel?.Invoke();
        _pickedBuilding = null;

        BuildingInformationBoard.GlobalBuildingInformationBoard.Get().HideInformation();

        gridRenderer.enabled = false;
    }
}
