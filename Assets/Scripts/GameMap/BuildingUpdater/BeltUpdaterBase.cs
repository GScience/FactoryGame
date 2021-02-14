using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class BeltUpdaterBase : IBuildingUpdater
{
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
        OnUpdate(GetBelt(building));
    }


    public abstract void OnBuilt(Belt building);
    public abstract void OnCrash(Belt building);
    public abstract void OnStart(Belt building);
    public abstract void OnStop(Belt building);
    public abstract void OnUpdate(Belt building);
}
