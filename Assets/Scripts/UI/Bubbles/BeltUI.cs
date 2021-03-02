using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 传送带UI
/// </summary>
[RequireComponent(typeof(Bubble))]
public class BeltUI : MonoBehaviour
{
    [HideInInspector]
    [NonSerialized]
    public Belt belt;

    /// <summary>
    /// 显示警告用的文本框
    /// </summary>
    public TextMeshProUGUI text;
    public ItemStackUI itemImage;

    void Update()
    {
        if (belt == null)
            return;

        itemImage.Item = belt.cargo;

        if (belt.GetInputBuilding() == null || belt.GetOutputBuilding() == null)
            text.text = "<color=#ff0000>" + LangManager.Current.Belt_Warning_No_Connection + "</color>";
        else
            text.text = "<color=#07b807>" + LangManager.Current.Building_Everything_Fine + "</color>";
    }
}
