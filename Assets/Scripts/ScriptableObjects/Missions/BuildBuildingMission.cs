using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "new Mission", menuName = "Game/Mission/Build Building")]
public class BuildBuildingMission : Mission
{
    public BuildingInfo building;

    public override string Prefix => "任务：";
    public override bool ShowMissionFinish => true;

    public override bool Check()
    {
        if (!GameManager.StatsSystem.BuildingCount.TryGetValue(building, out var value))
            return false;
        return value > 0;
    }
}
