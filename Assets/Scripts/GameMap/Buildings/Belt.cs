using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 传送带
/// </summary>
public class Belt : BuildingBase, IBuildingCanInputItem, IBuildingCanOutputItem
{
    /// <summary>
    /// 传送带上的物品
    /// </summary>
    [NonSerialized]
    [HideInInspector]
    public ItemInfo cargo;

    /// <summary>
    /// 运送的百分比
    /// </summary>
    [NonSerialized]
    [HideInInspector]
    public float percentage;

    public override void OnMouseEnter()
    {

    }

    public override void OnMouseLeave()
    {

    }

    public ItemInfo TakeAnyOneItem()
    {
        var item = cargo;
        cargo = null;
        return item;
    }

    public bool TryPutOneItem(ItemInfo item)
    {
        if (cargo != null)
            return false;
        cargo = item;
        return true;
    }

    public bool TryTakeOneItem(ItemInfo item)
    {
        if (cargo == item)
        {
            cargo = null;
            return true;
        }
        return false;
    }
}
