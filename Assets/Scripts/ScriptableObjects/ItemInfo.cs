using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 物品定义
/// </summary>
[CreateAssetMenu(fileName="New Item", menuName = "Game/Item")]
public class ItemInfo : NamedScriptableObject
{
    public string itemName;
    public int maxStackCount = 1;
    public Sprite icon;
    public int basePrice;

    public void OnValidate()
    {
        if (maxStackCount < 1)
            maxStackCount = 1;
    }
}
