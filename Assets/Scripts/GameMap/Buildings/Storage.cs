using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 仓库
/// </summary>
public class Storage : BuildingBase, IBuildingCanOutputItem, IBuildingCanOutputToOther
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

    public void OutputTo(IBuildingCanInputItem building)
    {
        outputBuilding = building;
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

