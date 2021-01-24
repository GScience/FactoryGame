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
    public Text factoryNameText;
    private Collapse _collapse;

    void Awake()
    {
        GlobalBuildingInformationBoard = new InstanceHelper<BuildingInformationBoard>(this);
        _collapse = GetComponent<Collapse>();
    }

    public void ShowInformation(string factoryName)
    {
        _collapse.Open();
        factoryNameText.text = factoryName;
    }

    public void HideInformation()
    {
        _collapse.Close();
    }
}
