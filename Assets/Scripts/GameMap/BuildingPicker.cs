using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 建筑选择器
/// 移动、建造建筑时让建筑在鼠标位置
/// </summary>
public class BuildingPicker : MonoBehaviour
{
    public static InstanceHelper<BuildingPicker> GlobalPicker;

    private Factory _pickedFactory;
    private Action _onConfirm;
    private Action _onCancel;

    void Awake()
    {
        GlobalPicker = new InstanceHelper<BuildingPicker>(this);
    }

    void Update()
    {
        if (_pickedFactory == null)
            return;
        
        var viewportPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _pickedFactory.transform.position = new Vector3(viewportPos.x, viewportPos.y, 1);

        if (Input.GetMouseButton(0))
            OnConfirm();
        else if (Input.GetKey(KeyCode.Escape))
            OnCancel();
    }

    public void Pick(Factory obj, Action onConfirm, Action onCancel)
    {
        if (_pickedFactory != null)
            OnCancel();

        _pickedFactory = obj;
        _onConfirm = onConfirm;
        _onCancel = onCancel;

        BuildingInformationBoard.GlobalBuildingInformationBoard.Get().ShowInformation(_pickedFactory.factoryName);
    }

    void OnConfirm()
    {
        if (_pickedFactory == null)
            return;
        _onConfirm?.Invoke();
        _pickedFactory = null;

        BuildingInformationBoard.GlobalBuildingInformationBoard.Get().HideInformation();
    }

    void OnCancel()
    {
        if (_pickedFactory == null)
            return;
        _onCancel?.Invoke();
        _pickedFactory = null;

        BuildingInformationBoard.GlobalBuildingInformationBoard.Get().HideInformation();
    }
}
