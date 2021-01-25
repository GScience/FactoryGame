using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FactoryCard : MonoBehaviour, IPointerClickHandler
{
    public Image factoryIconImage;
    public Text factoryNameText;

    public Building building;

    public void Start()
    {
        factoryIconImage.sprite = building.icon;
        factoryNameText.text = building.buildingName;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var selector = BuildingElementSelector.GlobalSelector.Get();

        selector.Close();

        var factory = Instantiate(building.gameObject, GameMap.GlobalMap.Get().transform);
        FactoryBuilder.GlobalBuilder.Get().Pick(factory.GetComponent<Building>(), 
            () =>
            {
                selector.Open();
            }, 
            () =>
            {
                Destroy(factory);
                selector.Open();
            });
    }
}
