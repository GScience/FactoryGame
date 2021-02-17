using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingCard : BuildingCardBase
{
    protected override void Build()
    {
        BeginBuild();

        var factory = GameMap.GlobalMap.Get().CreateBuildingPreview(building);

        BuildingBuilder.GlobalBuilder.Get().Pick(factory.GetComponent<BuildingBase>(),
            FinishBuild,
            () =>
            {
                Destroy(factory.gameObject);
                FinishBuild();
            });
    }
}
