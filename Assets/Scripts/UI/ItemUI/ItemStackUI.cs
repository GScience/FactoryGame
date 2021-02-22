using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 物品堆UI
/// </summary>
public class ItemStackUI : MonoBehaviour
{
    private Image _itemIconImage;
    private TextMeshProUGUI _itemCountText;

    public ItemInfo Item
    {
        set
        {
            if (value == null)
                _itemIconImage.enabled = false;
            else
            {
                _itemIconImage.sprite = value.icon;
                _itemIconImage.enabled = true;
            }
            _itemCountText.text = "";
        }
    }

    public ItemStack ItemStack
    {
        set
        {
            if (value == null)
                _itemIconImage.enabled = false;
            else
            {
                _itemIconImage.sprite = value.item.icon;
                _itemCountText.text = "" + value.count;
                _itemIconImage.enabled = true;
            }
        }
    }

    void Awake()
    {
        _itemIconImage = GetComponentInChildren<Image>();
        _itemCountText = GetComponentInChildren<TextMeshProUGUI>();

        Item = null;
    }
}
