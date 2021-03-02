using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 传送带分配器
/// 不允许主动放物品
/// </summary>
public class Distributor : BuildingBase, IBuildingCanInputItem, IBuildingCanOutputItem
{
    /// <summary>
    /// 接口类型
    /// </summary>
    public enum PortType
    {
        Disabled = 0, 
        In = 1, 
        Out = 2,
        Unknown = 3
    }

    private PortType[] _portsType = new PortType[4];
    private object[] _portsObj = new object[4];

    private ItemInfo _itemCache;

    /// <summary>
    /// 上一个输入的端口
    /// </summary>
    private int _lastInputPort;

    /// <summary>
    /// 上一个输出的端口
    /// </summary>
    private int _lastOutputPort;

    public void BeginDistributeItem()
    {
        if (_itemCache == null)
            UpdateInput();
        else
            UpdateOutput();
    }

    public PortType GetPortType(Vector2Int direction)
    {
        var id = DirectionToPortID(direction);
        return _portsType[id];
    }

    public void SetPortType(Vector2Int direction, PortType portType)
    {
        var id = DirectionToPortID(direction);
        if (_portsType[id] == portType)
            return;

        if (_portsType[id] != PortType.Disabled)
            DisablePort(id);

        _portsType[id] = portType;
    }

    private void UpdateInput()
    {
        int checkTime = 0;
        do
        {
            ++checkTime;
            if (checkTime >= 4)
                return;
            ++_lastInputPort;
            if (_lastInputPort >= 4)
                _lastInputPort = 0;
        } while (_portsType[_lastInputPort] != PortType.In);

        // 获取到建筑
        var building = _portsObj[_lastInputPort] as IBuildingCanOutputItem;
        if (building == null)
            return;

        // 尝试拿东西
        var item = building.TakeAnyOneItem();
        if (item == null)
            return;
        _itemCache = item;
    }

    private void UpdateOutput()
    {
        int checkTime = 0;
        do
        {
            ++checkTime;
            if (checkTime >= 4)
                return;
            ++_lastOutputPort;
            if (_lastOutputPort >= 4)
                _lastOutputPort = 0;
        } while (_portsType[_lastOutputPort] != PortType.Out);

        // 获取到建筑
        var building = _portsObj[_lastOutputPort] as IBuildingCanInputItem;
        if (building == null)
            return;

        // 尝试放入建筑
        if (building.TryPutOneItem(_itemCache))
            _itemCache = null;
    }

    /// <summary>
    /// 把方向转换成端口的Id
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private int DirectionToPortID(Vector2Int direction)
    {
        if (direction.x == 1)
            return 0;
        if (direction.x == -1)
            return 1;
        if (direction.y == 1)
            return 2;
        if (direction.y == -1)
            return 3;
        return -1;
    }

    public override void OnClick()
    {
        var popMenu = PopMenuLayer.GlobalPopMenuLayer.Get().Pop("DistributorPopMenu");
        popMenu.GetComponent<DistributorPopMenu>().distributor = this;
    }

    public override void OnMouseEnter()
    {
        
    }

    public override void OnMouseLeave()
    {
        
    }

    public override void OnPlace()
    {
        
    }

    public bool CanSetInputFrom(IBuildingCanOutputItem building, Vector2Int inputPos)
    {
        if (building == null)
            return true;
        var direction = GetRelevantPos(inputPos);
        var id = DirectionToPortID(direction);
        return _portsObj[id] == null && _portsType[id] == PortType.In;
    }

    public bool CanSetOutputTo(IBuildingCanInputItem building, Vector2Int outputPos)
    {
        if (building == null)
            return true;
        var direction = GetRelevantPos(outputPos);
        var id = DirectionToPortID(direction);
        return _portsObj[id] == null && _portsType[id] == PortType.Out;
    }

    public IBuildingCanOutputItem[] GetInputBuildings()
    {
        var list = new List<IBuildingCanOutputItem>();
        for (var i = 0; i < 4; ++i)
            if (_portsType[i] == PortType.In)
                list.Add(_portsObj[i] as IBuildingCanOutputItem);

        return list.ToArray();
    }

    public IBuildingCanInputItem[] GetOutputBuildings()
    {
        var list = new List<IBuildingCanInputItem>();
        for (var i = 0; i < 4; ++i)
            if (_portsType[i] == PortType.Out)
                list.Add(_portsObj[i] as IBuildingCanInputItem);

        return list.ToArray();
    }

    public ItemInfo TakeAnyOneItem()
    {
        return null;
    }

    public bool TryPutOneItem(ItemInfo item)
    {
        return false;
    }

    private void DisablePort(int id)
    {
        if (_portsType[id] == PortType.Disabled)
            return;

        // 断开连接
        if (_portsObj[id] != null)
        {
            var pos = GetComponent<GridElement>().CellPos;

            if (_portsType[id] == PortType.In)
                (_portsObj[id] as IBuildingCanOutputItem).TrySetOutputTo(null, pos);
            else
                (_portsObj[id] as IBuildingCanInputItem).TrySetInputFrom(null, pos);
        }
        _portsObj[id] = null;
        _portsType[id] = PortType.Disabled;
    }

    public bool TrySetInputFrom(IBuildingCanOutputItem building, Vector2Int inputPos)
    {
        var direction = GetRelevantPos(inputPos);
        var id = DirectionToPortID(direction);
        if (building == null)
        {
            DisablePort(id);
            return true;
        }
        if (_portsType[id] != PortType.In)
            return false;
        if (_portsObj[id] != null)
            return false;

        _portsObj[id] = building;
        return true;
    }

    public bool TrySetOutputTo(IBuildingCanInputItem building, Vector2Int outputPos)
    {
        var direction = GetRelevantPos(outputPos);
        var id = DirectionToPortID(direction);
        if (building == null)
        {
            DisablePort(id);
            return true;
        }
        if (_portsType[id] != PortType.Out)
            return false;
        if (_portsObj[id] != null)
            return false;

        _portsObj[id] = building;
        return true;
    }

    public bool TryTakeOneItem(ItemInfo item)
    {
        return false;
    }
}
