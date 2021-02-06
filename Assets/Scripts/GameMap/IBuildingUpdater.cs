using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IBuildingUpdater
{
    /// <summary>
    /// 当机械因为特殊原因崩溃
    /// </summary>
    void OnCrash(BuildingBase building);

    /// <summary>
    /// 当机械被建造
    /// </summary>
    void OnBuilt(BuildingBase building);

    /// <summary>
    /// 当机器关机
    /// </summary>
    void OnStop(BuildingBase building);

    /// <summary>
    /// 当机器开机
    /// </summary>
    void OnStart(BuildingBase building);

    /// <summary>
    /// 当刷新建筑
    /// </summary>
    void OnUpdate(BuildingBase building);
}
