using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 传送带刷新基类
/// </summary>
public abstract class BeltUpdaterBase : IBuildingUpdater
{
    /// <summary>
    /// 传送带刷新速度
    /// </summary>
    public abstract float Speed { get; }

    private static Belt GetBelt(BuildingBase building)
    {
        if (building is Belt factory)
            return factory;
        Debug.LogError("Not a belt");
        return null;
    }

    public void OnBuilt(BuildingBase building)
    {
        OnBuilt(GetBelt(building));
    }

    public void OnCrash(BuildingBase building)
    {
        OnCrash(GetBelt(building));
    }

    public void OnStart(BuildingBase building)
    {
        OnStart(GetBelt(building));
    }

    public void OnStop(BuildingBase building)
    {
        OnStop(GetBelt(building));
    }

    public void OnUpdate(BuildingBase building)
    {
        var belt = GetBelt(building);
        if (belt.cargo == null)
            return;

        belt.percentage += Time.deltaTime * Speed;

        if (belt.percentage > 1)
        {
            belt.percentage = 1;
            if (TryFinishItemCarrying(belt))
            {
                belt.cargo = null;
                belt.percentage = 0;
            }
        }

        OnCarryingItem(belt, belt.percentage);
    }

    /// <summary>
    /// 当运送物品时调用
    /// </summary>
    /// <param name="process"></param>
    public abstract void OnCarryingItem(Belt building, float process);

    /// <summary>
    /// 尝试结束
    /// </summary>
    public abstract bool TryFinishItemCarrying(Belt building);

    public abstract void OnBuilt(Belt building);
    public abstract void OnCrash(Belt building);
    public abstract void OnStart(Belt building);
    public abstract void OnStop(Belt building);
}
