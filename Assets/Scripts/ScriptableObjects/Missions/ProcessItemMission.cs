using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "new Mission", menuName = "Game/Mission/Process Item")]
public class ProcessItemMission : Mission
{
    public ItemInfo item;
    public int count;

    public override string Prefix => "任务：";
    public override bool ShowMissionFinish => true;

    public override string MissionState
    {
        get
        {
            if (!GameManager.StatsSystem.ProcessItemCount.TryGetValue(item, out var value))
                value = 0;
            return item.itemName + " " + value + "/" + count;
        }
    }

    public override bool Check()
    {
        if (!GameManager.StatsSystem.ProcessItemCount.TryGetValue(item, out var value))
            return false;
        return value >= count;
    }
}
