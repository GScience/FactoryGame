using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 工厂
/// 有配方和可以进行加工的建筑
/// </summary>
public class Factory : BuildingBase, IAcceptItemBuilding
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
    [HideInInspector]
    [NonSerialized]
    public RecipeInfo currentRecipe;

    // 输入物品缓存
    public List<ItemStack> inputItemCache;

    // 输出物品缓存
    public List<ItemStack> outputItemCache;

    /// <summary>
    /// 尝试进行加工
    /// 如果容器内物品不够则不加工
    /// </summary>
    /// <returns></returns>
    public bool TryManufacturing()
    {
        // 计算缺少物品
        var isItemEnough = true;
        foreach (var itemStackRequire in currentRecipe.requirement)
        {
            var result = inputItemCache.FirstOrDefault(itemStack => itemStack.item == itemStackRequire.item);
            var count = result?.count ?? 0;

            // 尝试拿物品
            if (count < itemStackRequire.count)
            {
                if (InputBuilding == null)
                    isItemEnough = false;
                else
                    for (var i = count; i < itemStackRequire.count; ++i)
                    {
                        if (!InputBuilding.TryTakeItem(itemStackRequire.item))
                        {
                            isItemEnough = false;
                            break;
                        }

                        ++count;
                    }
            }

            if (result != null)
                result.count = count;
            else if (count != 0)
                inputItemCache.Add(new ItemStack {count = count, item = itemStackRequire.item});
        }

        if (isItemEnough)
        {
            processingStartTime = Time.time;
            Debug.Log("Factory " + id + " satisfy the condition");
        }

        return isItemEnough;
    }

    /// <summary>
    /// 是否正在加工
    /// </summary>
    public bool IsManufacturing => processingStartTime > 0;

    public IAcceptItemBuilding InputBuilding { get; set; }

    /// <summary>
    /// 完成加工
    /// </summary>
    public void FinishManufacturing()
    {
        Debug.Log("Factory " + id + " finished");

        currentRecipe.output.ForEach((i) => outputItemCache.Add(new ItemStack {item = i.item, count = i.count}));
        inputItemCache.Clear();
        processingStartTime = -1;
    }

    public bool HasItem(Item item)
    {
        return outputItemCache.Any(itemStack => itemStack.item == item);
    }

    public bool TryTakeItem(Item item)
    {
        var result = outputItemCache.Count == 0 ? null : outputItemCache.FirstOrDefault(itemStack => itemStack.item == item);

        if (result == null)
            return false;

        --result.count;

        if (result.count == 0)
            outputItemCache.Remove(result);

        Debug.Log("Factory " + id + "'s item is taken");

        return true;
    }

    public Item TakeAnyItem()
    {
        if (outputItemCache.Count == 0)
            return null;

        var result = outputItemCache[0];

        --result.count;

        if (result.count == 0)
            outputItemCache.Remove(result);

        return result.item;
    }
}
