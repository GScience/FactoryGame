using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 出货箱
/// </summary>
public class SellingBox : BuildingBase, IBuildingCanInputItem
{
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
        if (relevantPos.x != 1 || relevantPos.y != 2)
            return false;
        return inputBuilding == null || building == null;
    }

    public IBuildingCanOutputItem GetInputBuilding()
    {
        return inputBuilding;
    }

    public bool TryPutOneItem(ItemInfo item)
    {
        return GameManager.GlobalGameManager.Get().TrySellItem(item);
    }
}

