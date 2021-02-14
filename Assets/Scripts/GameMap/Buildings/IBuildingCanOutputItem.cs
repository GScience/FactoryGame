using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 可向外输出物品的建筑
/// </summary>
public interface IBuildingCanOutputItem
{
    /// <summary>
    /// 拿走特定物品
    /// </summary>
    /// <param name="item"></param>
    bool TryTakeOneItem(ItemInfo item);

    /// <summary>
    /// 拿走任意物品
    /// </summary>
    /// <returns>返回为null代表无物品</returns>
    ItemInfo TakeAnyOneItem();
}
