using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 最终的任务，不可能实现
/// </summary>
[CreateAssetMenu(fileName = "new Mission", menuName = "Game/Mission/Finale")]
public class FinaleMission : Mission
{
    public override string Prefix => "";

    public override string MissionState => "";

    public override bool ShowMissionFinish => false;

    public override bool Check()
    {
        return false;
    }
}
