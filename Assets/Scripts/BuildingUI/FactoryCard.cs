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

public class FactoryCard : MonoBehaviour, IPointerClickHandler
{
    public Image factoryIconImage;
    public TextMeshProUGUI factoryNameText;

    public BuildingBase building;

    public void Start()
    {
        factoryIconImage.sprite = building.info.icon;
        factoryNameText.text = building.info.buildingName;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var selector = BuildingElementSelector.GlobalSelector.Get();

        selector.Close();

        var factory = GameMap.GlobalMap.Get().CreateBuildingPreview(building);
        
        BuildingBuilder.GlobalBuilder.Get().Pick(factory.GetComponent<BuildingBase>(), 
            () =>
            {
                selector.Open();
            }, 
            () =>
            {
                Destroy(factory.gameObject);
                selector.Open();
            });
    }
}
