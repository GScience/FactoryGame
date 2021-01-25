using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

/// <summary>
/// 地图原件，相当于所有Building的通用逻辑代码
/// </summary>
[RequireComponent(typeof(Renderer), typeof(Container), typeof(GridElement))]
public class Building : MonoBehaviour
{
    public string buildingName;
    [TextArea(20, 50)]
    public string buildingDescription;

    public int cost;
    public int people;

    public Sprite icon;
}
