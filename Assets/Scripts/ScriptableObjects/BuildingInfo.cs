using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

/// <summary>
/// 建筑信息
/// </summary>
[CreateAssetMenu(fileName="new Building", menuName="Game/Building")]
public class BuildingInfo : NamedScriptableObject
{
    public string buildingName;
    [TextArea(20, 50)]
    public string buildingDescription;

    public int cost;
    public int people;

    public Sprite icon;
}
