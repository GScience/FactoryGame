using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 显示选择的建筑的信息
/// </summary>
[RequireComponent(typeof(Collapse))]
public class BuildingInformationBoard : MonoBehaviour
{
    public static InstanceHelper<BuildingInformationBoard> GlobalBuildingInformationBoard;
    public Text buildingNameText;
    public Text buildingDescriptionText;
    public Text buildingCostText;
    public Text buildingPeopleText;
    public Image buildingIconImage;
    private Collapse _collapse;

    void Awake()
    {
        GlobalBuildingInformationBoard = new InstanceHelper<BuildingInformationBoard>(this);
        _collapse = GetComponent<Collapse>();
    }

    public void ShowInformation(Building building)
    {
        _collapse.Open();
        buildingNameText.text = building.buildingName;
        buildingDescriptionText.text = building.buildingDescription;
        buildingCostText.text = "" + building.cost;
        buildingPeopleText.text = "" + building.people;
        buildingIconImage.sprite = building.icon;
    }

    public void HideInformation()
    {
        _collapse.Close();
    }
}
