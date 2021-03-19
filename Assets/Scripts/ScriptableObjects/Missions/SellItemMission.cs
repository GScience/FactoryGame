using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "new Mission", menuName = "Game/Mission/Sell Item")]
public class SellItemMission : Mission
{
    public ItemInfo item;
    public int count;

    public override string Prefix => "任务：";
    public override bool ShowMissionFinish => true;

    public override bool Check()
    {
        if (!GameManager.StatsSystem.SellItemCount.TryGetValue(item, out var value))
            return false;
        return value >= count;
    }
}
