using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 可接受物品的建筑
/// </summary>
public interface IAcceptItemBuilding
{
    /// <summary>
    /// 是否包含指定物品
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    bool HasItem(Item item);

    /// <summary>
    /// 拿走特定物品
    /// </summary>
    /// <param name="item"></param>
    bool TryTakeItem(Item item);

    /// <summary>
    /// 拿走任意物品
    /// </summary>
    /// <returns>返回为null代表无物品</returns>
    Item TakeAnyItem();

    IAcceptItemBuilding InputBuilding { get; set; }
}
