using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class BeltCard : BuildingCardBase
{
    public Belt cw;
    public Belt ccw;

    protected override void Build()
    {
        BeginBuild();
        var straight = building.GetComponent<Belt>();
        BeltBuilder.GlobalBuilder.Get().ChooseBeltPrefabs(straight, cw, ccw, FinishBuild);
    }
}
