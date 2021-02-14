using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class FactoryUpdaterBase : IBuildingUpdater
{
    private static Factory GetFactory(BuildingBase building)
    {
        if (building is Factory factory)
            return factory;
        Debug.LogError("Not a factory");
        return null;
    }

    public void OnCrash(BuildingBase building)
    {
        OnCrash(GetFactory(building));
    }

    public void OnBuilt(BuildingBase building)
    {
        OnBuilt(GetFactory(building));
    }

    public void OnStop(BuildingBase building)
    {
        OnStop(GetFactory(building));
    }

    public void OnStart(BuildingBase building)
    {
        OnStart(GetFactory(building));
    }

    public void OnUpdate(BuildingBase building)
    {
        var factory = GetFactory(building);

        // 处理生产
        if (factory.CurrentRecipe == null || !factory.IsManufacturing) 
            return;

        var deltaTime = Time.time - factory.processingStartTime;
        if (deltaTime > factory.CurrentRecipe.time)
            factory.FinishManufacturing();
        else
            OnProcessing(factory, deltaTime / factory.CurrentRecipe.time);

        // 处理输出
        factory.TryPopItem();
    }

    /// <summary>
    /// 当加工生产
    /// </summary>
    /// <param name="percent">生产百分比</param>
    public abstract void OnProcessing(Factory factory, float percent);

    public abstract void OnCrash(Factory factory);
    public abstract void OnBuilt(Factory factory);
    public abstract void OnStop(Factory factory);
    public abstract void OnStart(Factory factory);
}
