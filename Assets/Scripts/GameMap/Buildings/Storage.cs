using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 仓库
/// </summary>
public class Storage : BuildingBase, IBuildingCanOutputItem
{
    public ItemInfo itemInfo;

    public IBuildingCanInputItem outputBuilding;

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

    public bool TrySetOutputTo(IBuildingCanInputItem building, Vector2Int inputPos)
    {
        if (!CanSetOutputTo(building, inputPos))
            return false;
        outputBuilding = building;
        return true;
    }

    public bool CanSetOutputTo(IBuildingCanInputItem building, Vector2Int inputPos)
    {
        var relevantPos = GetRelevantPos(inputPos);
        if (relevantPos.x != 1 || relevantPos.y != -1)
            return false;
        return outputBuilding == null || building == null;
    }

    public IBuildingCanInputItem[] GetOutputBuildings()
    {
        if (outputBuilding == null)
            return new IBuildingCanInputItem[] { };
        return new[] { outputBuilding };
    }

    public ItemInfo TakeAnyOneItem()
    {
        if (CanTakeItem())
        {
            GameManager.MoneySystem.RequireMoney(itemInfo.basePrice);
            return itemInfo;
        }
        return null;
    }

    public bool TryTakeOneItem(ItemInfo item)
    {
        if (CanTakeItem())
        {
            GameManager.MoneySystem.RequireMoney(itemInfo.basePrice);
            return itemInfo == item;
        }
        return false;
    }

    public bool CanTakeItem()
    {
        return GameManager.MoneySystem.HasEnoughMoney(itemInfo.basePrice);
    }

    public override void Save(BinaryWriter writer)
    {
        SaveHelper.Write(writer, itemInfo);
        SaveHelper.Write(writer, outputBuilding);
    }

    public override void Load(BinaryReader reader)
    {
        itemInfo = SaveHelper.ReadScriptable<ItemInfo>(reader);
        outputBuilding = SaveHelper.ReadBuildingCanInput(reader);
    }
}

