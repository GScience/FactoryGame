using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 分配器刷新器
/// </summary>
public class DistributorUpdater : IBuildingUpdater
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
        var distributor = building as Distributor;

        distributor.BeginDistributeItem();
    }
}
