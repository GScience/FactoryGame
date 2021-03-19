using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 单纯的显示提示
/// </summary>
[CreateAssetMenu(fileName = "new Tip", menuName = "Game/Mission/Tip")]
public class TipMission : Mission
{
    public override string Prefix => "提示：";
    public override bool ShowMissionFinish => false;

    public override bool Check()
    {
        return true;
    }
}
