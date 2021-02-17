using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class BuildingCardBase : MonoBehaviour, IPointerClickHandler
{
    public Image factoryIconImage;
    public TextMeshProUGUI factoryNameText;

    public BuildingBase building;

    private BuildingElementSelector _selector;

    public void Start()
    {
        factoryIconImage.sprite = building.info.icon;
        factoryNameText.text = building.info.buildingName;
        _selector = BuildingElementSelector.GlobalSelector.Get();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Build();
    }

    public void BeginBuild()
    {
        _selector.Close();
    }

    public void FinishBuild()
    {
        _selector.Open();
    }

    protected abstract void Build();
}
