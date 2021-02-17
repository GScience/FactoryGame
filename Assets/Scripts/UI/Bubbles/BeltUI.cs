using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    public ItemStackUI itemImage;

    void Update()
    {
        if (belt == null)
            return;

        itemImage.Item = belt.cargo;
    }
}
