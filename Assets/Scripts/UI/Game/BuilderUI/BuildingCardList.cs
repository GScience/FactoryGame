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
        var cards = ResourcesManager.GetAllCards();
        foreach (var card in cards)
            Instantiate(card, transform);
    }
}
