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
    private TextMeshProUGUI _itemNameText;

    public ItemInfo Item
    {
        set
        {
            if (value == null)
            {
                _itemIconImage.enabled = false;
                _itemNameText.text = "";
            }
            else
            {
                _itemIconImage.sprite = value.icon;
                _itemNameText.text = value.itemName;
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
            {
                _itemIconImage.enabled = false;
                _itemNameText.text = "";
            }
            else
            {
                _itemIconImage.sprite = value.item.icon;
                _itemCountText.text = "" + value.count;
                _itemNameText.text = value.item.itemName;
                _itemIconImage.enabled = true;
            }
        }
    }

    void Awake()
    {
        _itemIconImage = GetComponentInChildren<Image>();
        var texts = GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var text in texts)
        {
            if (text.name == "Count")
                _itemCountText = text;
            else if (text.name == "Name")
                _itemNameText = text;
        }

        Item = null;
    }
}
