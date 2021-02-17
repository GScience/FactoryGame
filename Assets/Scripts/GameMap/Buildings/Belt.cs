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
    public enum BeltState
    {
        Right2Left = 0,
        Up2Down = 1,
        Left2Right = 2,
        Down2Up = 3,

        Down2Left = 4,
        Right2Down = 5,
        Up2Right = 6,
        Left2Up = 7,

        Down2Right = 8,
        Right2Up = 9,
        Up2Left = 10,
        Left2Down = 11
    }

    public enum BeltType
    {
        Straight = 0,
        CornerCcw = 1,
        CornerCw = 2
    }

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
    /// 传送带类型
    /// </summary>
    public BeltType beltType;

    /// <summary>
    /// 是否可见
    /// </summary>
    private bool _isVisible;

    public BeltState State
    {
        get => _state;
        set => SetBeltState(value);
    }
    private BeltState _state;

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
        if (PlayerInput.IsBuilding())
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

    public override void OnPlace()
    {
        
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

    public override void ChangeToState(int offset)
    {
        offset %= 4;
        var offsetState = (int)_state - (int) beltType * 4 + offset;
        if (offsetState < 0)
            offsetState += 4;
        var value = offsetState % 4 + (int) beltType * 4;
        SetBeltState((BeltState) value);
    }

    public void SetBeltState(BeltState state)
    {
        if (_state != state)
            transform.rotation = Quaternion.AngleAxis(((int)state % 4) * 90, Vector3.forward);

        _state = state;
    }

    public void SetBeltDirection(Vector2Int inDirection, Vector2Int outDirection)
    {
        if (beltType == BeltType.Straight)
            Debug.LogError("Only corner belt support");
    }

    private void SetStraightDirection(Vector2Int direction)
    {
        switch (direction.x)
        {
            case 1:
                SetBeltState(BeltState.Left2Right);
                break;
            case -1:
                SetBeltState(BeltState.Right2Left);
                break;
        }
        switch (direction.y)
        {
            case 1:
                SetBeltState(BeltState.Down2Up);
                break;
            case -1:
                SetBeltState(BeltState.Up2Down);
                break;
        }
    }

    private void SetCwDirection(Vector2Int direction)
    {
        switch (direction.x)
        {
            case 1:
                SetBeltState(BeltState.Left2Down);
                break;
            case -1:
                SetBeltState(BeltState.Right2Up);
                break;
        }
        switch (direction.y)
        {
            case 1:
                SetBeltState(BeltState.Down2Right);
                break;
            case -1:
                SetBeltState(BeltState.Up2Left);
                break;
        }
    }

    private void SetCcwDirection(Vector2Int direction)
    {
        switch (direction.x)
        {
            case 1:
                SetBeltState(BeltState.Left2Up);
                break;
            case -1:
                SetBeltState(BeltState.Right2Down);
                break;
        }
        switch (direction.y)
        {
            case 1:
                SetBeltState(BeltState.Down2Left);
                break;
            case -1:
                SetBeltState(BeltState.Up2Right);
                break;
        }
    }

    public void SetBeltDirection(Vector2Int direction)
    {
        switch (beltType)
        {
            case BeltType.Straight:
                SetStraightDirection(direction);
                break;
            case BeltType.CornerCw:
                SetCwDirection(direction);
                break;
            case BeltType.CornerCcw:
                SetCcwDirection(direction);
                break;
        }
    }
}
