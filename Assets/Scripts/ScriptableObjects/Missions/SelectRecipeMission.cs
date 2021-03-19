using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "new Mission", menuName = "Game/Mission/Select Recipe")]
public class SelectRecipeMission : Mission
{
    public RecipeInfo recipe;

    public override string Prefix => "任务：";
    public override bool ShowMissionFinish => true;

    public override string MissionState
    {
        get
        {
            if (!GameManager.StatsSystem.RecipeUseCount.TryGetValue(recipe, out var value))
                return "未设置";
            return "已设置";
        }
    }

    public override bool Check()
    {
        if (!GameManager.StatsSystem.RecipeUseCount.TryGetValue(recipe, out var value))
            return false;
        return value > 0;
    }
}
