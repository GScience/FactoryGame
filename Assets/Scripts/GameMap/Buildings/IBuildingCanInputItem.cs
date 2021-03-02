using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 可接受其他建筑输入物品的建筑
/// </summary>
public interface IBuildingCanInputItem
{
    /// <summary>
    /// 存入特定物品
    /// </summary>
    /// <param name="item"></param>
    bool TryPutOneItem(ItemInfo item);

    bool TrySetInputFrom(IBuildingCanOutputItem building, Vector2Int inputPos);
    bool CanSetInputFrom(IBuildingCanOutputItem building, Vector2Int inputPos);

    IBuildingCanOutputItem[] GetInputBuildings();
}
