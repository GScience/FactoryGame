using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BuildingCardList : MonoBehaviour
{
    private void Start()
    {
        var cards = GameManager.StageSystem.GetAllEnabledBuildingCard();
        foreach (var card in cards)
            Instantiate(card, transform);

        GameManager.StageSystem.OnEnableBuildingCard += (card) =>
        {
            Instantiate(card, transform);
        };
    }
}
