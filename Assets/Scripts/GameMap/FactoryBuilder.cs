using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

    void Awake()
    {
        GlobalBuilder = new InstanceHelper<FactoryBuilder>(this);
    }

    void Update()
    {
        if (_pickedBuilding == null)
            return;
        
        var viewportPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _pickedBuilding.transform.position = new Vector3(viewportPos.x, viewportPos.y, 1);

        if (Input.GetMouseButton(0))
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

        BuildingInformationBoard.GlobalBuildingInformationBoard.Get().ShowInformation(_pickedBuilding);
    }

    void OnConfirm()
    {
        if (_pickedBuilding == null)
            return;
        _onConfirm?.Invoke();
        _pickedBuilding = null;

        BuildingInformationBoard.GlobalBuildingInformationBoard.Get().HideInformation();
    }

    void OnCancel()
    {
        if (_pickedBuilding == null)
            return;
        _onCancel?.Invoke();
        _pickedBuilding = null;

        BuildingInformationBoard.GlobalBuildingInformationBoard.Get().HideInformation();
    }
}
