using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 传送带
/// </summary>
public class Belt : BuildingBase, IBuildingCanInputItem, IBuildingCanOutputItem
{
    /// <summary>
    /// 传送带上的物品
    /// </summary>
    [NonSerialized]
    [HideInInspector]
    public ItemInfo cargo;

    /// <summary>
    /// 显示运送的物品的精灵
    /// </summary>
    public SpriteRenderer cargoSpriteRenderer;

    /// <summary>
    /// 是否为拐弯处
    /// </summary>
    public bool isCorner;

    /// <summary>
    /// 是否可见
    /// </summary>
    private bool _isVisible;

    /// <summary>
    /// 运送的百分比
    /// </summary>
    [NonSerialized]
    [HideInInspector]
    public float percentage;

    private Bubble _popedUI;

    /// <summary>
    /// 输出建筑
    /// </summary>
    public IBuildingCanInputItem outputBuilding;

    public override void OnMouseEnter()
    {
        if (BuildingBuilder.GlobalBuilder.Get().IsBuilding)
            return;

        _popedUI = BubbleUILayer.GlobalBubbleUILayer.Get().Pop("BeltUI");
        _popedUI.GetComponent<BeltUI>().belt = this;
    }

    public override void OnMouseLeave()
    {
        if (_popedUI != null)
        {
            BubbleUILayer.GlobalBubbleUILayer.Get().Close(_popedUI);
            _popedUI = null;
        }
    }

    public ItemInfo TakeAnyOneItem()
    {
        var item = cargo;
        cargo = null;
        return item;
    }

    public bool TryPutOneItem(ItemInfo item)
    {
        if (cargo != null)
            return false;
        cargo = item;
        return true;
    }

    public bool TryTakeOneItem(ItemInfo item)
    {
        if (cargo == item)
        {
            cargo = null;
            return true;
        }
        return false;
    }

    void Update()
    {
        if (!_isVisible)
            return;

        if (cargo == null)
        {
            cargoSpriteRenderer.sprite = null;
            return;
        }
        cargoSpriteRenderer.sprite = cargo.icon;
        cargoSpriteRenderer.transform.localPosition =
            Vector2.Lerp(new Vector2(1, 0.5f), new Vector2(0, 0.5f), percentage) 
            - (Vector2)_renderer.bounds.size / 2 
            - Vector2.up * cargoSpriteRenderer.size.y / 3;
    }

    void OnBecameVisible()
    {
        _isVisible = true;
    }

    void OnBecameInvisible()
    {
        _isVisible = false;
    }
}
