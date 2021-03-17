using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 出货箱
/// </summary>
public class SellingBox : BuildingBase, IBuildingCanInputItem, IBuildingAutoConnect
{
    public static readonly Vector2Int InputPos = new Vector2Int(1, 2);

    public IBuildingCanOutputItem inputBuilding;

    public override void OnClick()
    {

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

    public bool TrySetInputFrom(IBuildingCanOutputItem building, Vector2Int inputPos)
    {
        if (!CanSetInputFrom(building, inputPos))
            return false;
        inputBuilding = building;
        return true;
    }

    public bool CanSetInputFrom(IBuildingCanOutputItem building, Vector2Int inputPos)
    {
        var relevantPos = GetRelevantPos(inputPos);
        if (relevantPos != InputPos)
            return false;
        return inputBuilding == null || building == null;
    }

    public IBuildingCanOutputItem[] GetInputBuildings()
    {
        if (inputBuilding == null)
            return new IBuildingCanOutputItem[] { };
        return new[] { inputBuilding };
    }

    public bool TryPutOneItem(ItemInfo item)
    {
        return GameManager.MoneySystem.TrySellItem(item);
    }

    public override void Save(BinaryWriter writer)
    {
        SaveHelper.Write(writer, inputBuilding);
    }

    public override void Load(BinaryReader reader)
    {
        inputBuilding = SaveHelper.ReadBuildingCanOutput(reader);
    }

    public void Reconnect()
    {
        var pos = _gridElement.CellPos;

        // 寻找输入
        var foundBuilding = GameMap.GlobalMap.Get().GetBuildingAt(pos + InputPos);
        if (foundBuilding is IBuildingCanOutputItem foundOutputBuilding)
        {
            // 尝试连接
            if (foundOutputBuilding.TrySetOutputTo(this, pos + InputPos + Vector2Int.down))
                inputBuilding = foundOutputBuilding;
        }
        else
            inputBuilding = null;
    }
}

