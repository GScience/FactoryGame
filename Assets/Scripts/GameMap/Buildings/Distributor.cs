using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 传送带分配器
/// 不允许主动放物品
/// </summary>
public class Distributor : BuildingBase, IBuildingCanInputItem, IBuildingCanOutputItem, IBuildingAutoConnect
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

    /// <summary>
    /// 传输百分比
    /// </summary>
    [NonSerialized]
    [HideInInspector]
    public float percent;

    public void BeginDistributeItem()
    {
        if (_itemCache == null)
            UpdateInput();
        else
        {
            if (percent < 1)
                percent += Time.deltaTime;
            else
            {
                UpdateOutput();
                if (_itemCache == null)
                    percent = 0;
            }
        }
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
        ReconnectPort(direction);
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
        if (_itemCache == null || percent < 1)
            return null;
        var tmpItem = _itemCache;
        _itemCache = null;
        return tmpItem;
    }

    public bool TryPutOneItem(ItemInfo item)
    {
        if (_itemCache != null)
            return false;
        _itemCache = item;
        return true;
    }

    private void DisablePort(int id)
    {
        if (_portsType[id] == PortType.Disabled)
            return;

        // 断开连接
        if (_portsObj[id] != null)
        {
            var pos = GetComponent<GridElement>().CellPos;

            if (_portsObj[id] != null)
            {
                if (_portsType[id] == PortType.In)
                    (_portsObj[id] as IBuildingCanOutputItem).TrySetOutputTo(null, pos);
                else
                    (_portsObj[id] as IBuildingCanInputItem).TrySetInputFrom(null, pos);
            }
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
            _portsObj[id] = null;
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
            _portsObj[id] = null;
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

    public override void Load(BinaryReader reader)
    {
        base.Load(reader);

        for (var i = 0; i < 4; ++i)
        {
            _portsType[i] = (PortType)reader.ReadChar();
            _portsObj[i] = SaveHelper.ReadBuilding(reader);
        }
        _itemCache = SaveHelper.ReadScriptable<ItemInfo>(reader);
        percent = reader.ReadSingle();
        _lastInputPort = reader.ReadChar();
        _lastOutputPort = reader.ReadChar();
    }

    public override void Save(BinaryWriter writer)
    {
        base.Save(writer);

        for (var i = 0; i < 4; ++i)
        {
            writer.Write((char)_portsType[i]);
            SaveHelper.Write(writer, _portsObj[i] as BuildingBase);
        }
        SaveHelper.Write(writer, _itemCache);
        writer.Write(percent);
        writer.Write((char)_lastInputPort);
        writer.Write((char)_lastOutputPort);
    }

    public void Reconnect()
    {
        ReconnectPort(Vector2Int.up);
        ReconnectPort(Vector2Int.down);
        ReconnectPort(Vector2Int.left);
        ReconnectPort(Vector2Int.right);
    }

    private void ReconnectPort(Vector2Int direction)
    {
        var id = DirectionToPortID(direction);
        var portType = _portsType[id];
        var pos = _gridElement.CellPos;
        var foundBuilding = GameMap.GlobalMap.Get().GetBuildingAt(pos + direction);

        switch (portType)
        {
            case PortType.In:
                if (foundBuilding is IBuildingCanOutputItem outputBuilding)
                {
                    if (outputBuilding.TrySetOutputTo(this, pos))
                        _portsObj[id] = outputBuilding;
                }
                else if (foundBuilding == null)
                    _portsObj[id] = null;
                break;
            case PortType.Out:
                if (foundBuilding is IBuildingCanInputItem inputBuilding)
                {
                    if (inputBuilding.TrySetInputFrom(this, pos))
                        _portsObj[id] = inputBuilding;
                }
                else if (foundBuilding == null)
                    _portsObj[id] = null;
                break;
        }
    }
}
