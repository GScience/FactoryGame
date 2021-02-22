using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class StorageUpdater : IBuildingUpdater
{
    public void OnBuilt(BuildingBase building)
    {
        
    }

    public void OnCrash(BuildingBase building)
    {
        
    }

    public void OnStart(BuildingBase building)
    {
        
    }

    public void OnStop(BuildingBase building)
    {
        
    }

    public void OnUpdate(BuildingBase building)
    {
        var storage = building as Storage;
        if (storage.outputBuilding != null)
            storage.outputBuilding.TryPutOneItem(storage.itemInfo);
    }
}
