using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 工厂
/// 有配方和可以进行加工的建筑
/// </summary>
public class Factory : BuildingBase, IBuildingCanInputItem, IBuildingCanOutputItem
{
    /// <summary>
    /// 加工开始的时间
    /// </summary>
    [HideInInspector]
    [NonSerialized]
    public float processingStartTime;

    /// <summary>
    /// 合成配方
    /// </summary>
    public RecipeInfo[] recipes;

    /// <summary>
    /// 当前所选配方
    /// </summary>
    public RecipeInfo CurrentRecipe { get; private set; }

    /// <summary>
    /// 输入物品缓存
    /// </summary>
    private ItemStack[] _inputItemCache;

    /// <summary>
    /// 输出物品缓存
    /// </summary>
    private ItemStack[] _outputItemCache;

    /// <summary>
    /// 是否正在加工
    /// </summary>
    public bool IsManufacturing
    {
        get => processingStartTime >= 0;
        private set => processingStartTime = value ? Time.time : -1;
    }

    /// <summary>
    /// 是否已经弹出所有物品
    /// </summary>
    public bool isAllItemPoped = true;

    /// <summary>
    /// 输出建筑
    /// </summary>
    public IBuildingCanInputItem outputBuilding;

    /// <summary>
    /// 尝试进行加工
    /// 如果容器内物品不够则不加工
    /// </summary>
    /// <returns></returns>
    private void TryManufacturing()
    {
        if (IsManufacturing)
            return;

        // 计算缺少物品
        if (_inputItemCache.Where((itemStack, i) => itemStack.count < CurrentRecipe.input[i].count).Any())
            return;

        // 开始加工
        IsManufacturing = true;
    }

    /// <summary>
    /// 当建筑物品刷新
    /// </summary>
    public void OnItemUpdate()
    {
        TryManufacturing();
    }

    /// <summary>
    /// 设置当前配方
    /// </summary>
    /// <param name="recipeId">配方ID</param>
    public void SetCurrentRecipe(int recipeId)
    {
        if (recipeId < 0 || recipeId >= recipes.Length)
        {
            Debug.LogError("Recipe id should between 0 to " + (recipes.Length - 1));
            return;
        }

        CurrentRecipe = recipes[recipeId];

        // 根据配方分配缓冲区大小
        _inputItemCache = new ItemStack[CurrentRecipe.input.Count];
        _outputItemCache = new ItemStack[CurrentRecipe.output.Count];

        // 设置对应的物品
        for (var i = 0; i < CurrentRecipe.input.Count; ++i)
        {
            _inputItemCache[i] = new ItemStack()
            {
                count = 0,
                item = CurrentRecipe.input[i].item
            };
        }

        for (var i = 0; i < CurrentRecipe.output.Count; ++i)
        {
            _outputItemCache[i] = new ItemStack()
            {
                count = 0,
                item = CurrentRecipe.output[i].item
            };
        }

        IsManufacturing = false;
    }

    /// <summary>
    /// 完成加工
    /// </summary>
    public void FinishManufacturing()
    {
        Debug.Log("Factory " + id + " finished");

        // 清空物品
        foreach (var item in _inputItemCache)
            item.count = 0;

        // 生产物品
        for (var i = 0; i < _outputItemCache.Length; ++i)
            _outputItemCache[i].count += CurrentRecipe.output[i].count;

        isAllItemPoped = false;
        IsManufacturing = false;
    }

    /// <summary>
    /// 尝试自动弹出物品
    /// </summary>
    public void TryPopItem()
    {
        if (isAllItemPoped || outputBuilding == null)
            return;

        for (var i = 0; i < _outputItemCache.Length;)
        {
            // 某种物品已经放完
            if (_outputItemCache[i].count <= 0)
            {
                ++i;
                continue;
            }

            // 某种物品放不下了
            if (!outputBuilding.TryPutOneItem(_outputItemCache[i].item))
                return;

            // 还是放下了
            --_outputItemCache[i].count;
        }

        isAllItemPoped = true;
    }

    public bool TryTakeOneItem(ItemInfo item)
    {
        foreach (var itemStack in _outputItemCache)
        {
            if (itemStack.item != item || itemStack.count <= 0)
                continue;
            --itemStack.count;
            return true;
        }

        return false;
    }

    public ItemInfo TakeAnyOneItem()
    {
        foreach (var itemStack in _outputItemCache)
        {
            if (itemStack.count <= 0)
                continue;
            --itemStack.count;
            return itemStack.item;
        }

        return null;
    }

    public bool TryPutOneItem(ItemInfo item)
    {
        for (var i = 0; i < _inputItemCache.Length;)
        {
            // 工厂的物品栏忽略堆叠
            ++_inputItemCache[i].count;
            OnItemUpdate();
            return true;
        }

        return false;
    }

    public int GetInputCacheCount(int id)
    {
        return _inputItemCache[id].count;
    }

    public float GetProgressPercentage()
    {
        if (!IsManufacturing || CurrentRecipe == null)
            return 0;

        return (Time.time - processingStartTime) / CurrentRecipe.time;
    }

    private Bubble _popedUI;

    public override void OnMouseEnter()
    {
        _popedUI = BubbleUILayer.GlobalBubbleUILayer.Get().Pop("FactoryManufacturingUI");
        _popedUI.GetComponent<FactoryUI>().factory = this;
    }

    public override void OnMouseLeave()
    {
        if (_popedUI != null)
        {
            BubbleUILayer.GlobalBubbleUILayer.Get().Close(_popedUI);
            _popedUI = null;
        }
    }
}
