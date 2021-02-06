using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 显示选择的建筑的信息
/// </summary>
[RequireComponent(typeof(Collapse))]
public class BuildingInformationBoard : MonoBehaviour
{
    public static InstanceHelper<BuildingInformationBoard> GlobalBuildingInformationBoard;
    public TextMeshProUGUI buildingNameText;
    public TextMeshProUGUI buildingDescriptionText;
    public TextMeshProUGUI buildingCostText;
    public TextMeshProUGUI buildingPeopleText;
    public Image buildingIconImage;
    private Collapse _collapse;

    void Awake()
    {
        GlobalBuildingInformationBoard = new InstanceHelper<BuildingInformationBoard>(this);
        _collapse = GetComponent<Collapse>();
    }

    public void ShowInformation(BuildingBase building)
    {
        _collapse.Open();
        buildingNameText.text = building.info.buildingName;
        buildingDescriptionText.text = building.info.buildingDescription;
        buildingCostText.text = "" + building.info.cost;
        buildingPeopleText.text = "" + building.info.people;
        buildingIconImage.sprite = building.info.icon;
    }

    public void HideInformation()
    {
        _collapse.Close();
    }
}
