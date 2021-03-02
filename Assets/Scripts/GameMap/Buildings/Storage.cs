using System;
using System.Collections.Generic;
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

    public IBuildingCanInputItem GetOutputBuilding()
    {
        return outputBuilding;
    }

    public ItemInfo TakeAnyOneItem()
    {
        return itemInfo;
    }

    public bool TryTakeOneItem(ItemInfo item)
    {
        return itemInfo == item;
    }
}

