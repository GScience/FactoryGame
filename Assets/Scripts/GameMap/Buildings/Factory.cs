using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 工厂
/// 有配方和可以进行加工的建筑
/// </summary>
public class Factory : BuildingBase, IBuildingCanInputItem, IBuildingCanOutputItem, IBuildingAutoConnect
{
    public static readonly Vector2Int[] InputPos = new[] { new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(-1, 2), new Vector2Int(1, -1), new Vector2Int(1, 3) };
    public static readonly Vector2Int OutputPos = new Vector2Int(3, 1);

    private static int GetInputPortIdFromRelPos(Vector2Int pos)
    {
        var id = Array.IndexOf(InputPos, pos);
        return id;
    }

    /// <summary>
    /// 加工经过的总时间
    /// </summary>
    [HideInInspector]
    [NonSerialized]
    public float processingTime;

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
        get => processingTime >= 0;
        private set => processingTime = value ? 0 : -1;
    }

    /// <summary>
    /// 是否已经弹出所有物品
    /// </summary>
    [HideInInspector]
    [NonSerialized]
    public bool isAllItemPoped = true;

    /// <summary>
    /// 机器是否被阻塞
    /// </summary>
    public bool IsStucked { get => !isAllItemPoped; }

    /// <summary>
    /// 输出建筑
    /// </summary>
    public IBuildingCanInputItem outputBuilding;

    /// <summary>
    /// 输入建筑
    /// </summary>
    public IBuildingCanOutputItem[] inputBuildings = new IBuildingCanOutputItem[InputPos.Length];

    /// <summary>
    /// 是否能放物品
    /// </summary>
    private bool? _canPlaceItem;

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
        var factoryInfo = info as FactoryInfo;
        if (factoryInfo == null)
            return;

        var recipes = factoryInfo.recipes;

        if (recipeId < 0)
        {
            if (CurrentRecipe != null)
            {
                _inputItemCache = null;
                _outputItemCache = null;
            }
            CurrentRecipe = null;
            return;
        }

        if (recipeId >= recipes.Length)
        {
            Debug.LogError("Recipe id should between negative to " + (recipes.Length - 1));
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
        _canPlaceItem = true;
    }

    /// <summary>
    /// 完成加工
    /// </summary>
    public void FinishManufacturing()
    {
        // 清空物品
        foreach (var item in _inputItemCache)
            item.count = 0;

        // 生产物品
        for (var i = 0; i < _outputItemCache.Length; ++i)
            _outputItemCache[i].count += CurrentRecipe.output[i].count;

        isAllItemPoped = false;
        IsManufacturing = false;
        _canPlaceItem = null;
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
        if (_outputItemCache == null)
            return false;

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
        if (_outputItemCache == null)
            return null;

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
        if (_canPlaceItem != null && _canPlaceItem == false)
            return false;

        if (_inputItemCache == null)
            return false;

        var isAllInputFilled = true;

        for (var i = 0; i < _inputItemCache.Length; ++i)
        {
            if (_inputItemCache[i].count >= CurrentRecipe.input[i].count)
                continue;
            isAllInputFilled = false;

            if (_inputItemCache[i].item != item)
                continue;
            ++_inputItemCache[i].count;
            OnItemUpdate();
            _canPlaceItem = true;
            return true;
        }

        if (isAllInputFilled)
            _canPlaceItem = false;
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

        return processingTime / CurrentRecipe.time;
    }

    private Bubble _popedUI;

    public override void OnMouseEnter()
    {
        if (PlayerInput.IsBuilding())
            return;

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

    public override void OnClick()
    {
        var popMenu = PopMenuLayer.GlobalPopMenuLayer.Get().Pop("FactoryPopMenu");
        popMenu.GetComponent<FactoryPopMenu>().factory = this;
    }

    public override void OnPlace()
    {
        
    }

    public bool TrySetOutputTo(IBuildingCanInputItem building, Vector2Int inputPos)
    {
        if (!CanSetOutputTo(building, inputPos))
            return false;
        outputBuilding = building;
        return true;
    }

    public bool CanSetOutputTo(IBuildingCanInputItem building, Vector2Int inputPos)
    {
        var relevantPos = GetRelevantPos(inputPos);
        if (relevantPos != OutputPos)
            return false;
        return outputBuilding == null || building == null;
    }

    public bool TrySetInputFrom(IBuildingCanOutputItem building, Vector2Int inputPos)
    {
        if (!CanSetInputFrom(building, inputPos))
            return false;
        var relevantPos = GetRelevantPos(inputPos);
        var id = GetInputPortIdFromRelPos(relevantPos);
        inputBuildings[id] = building;
        return true;
    }

    public bool CanSetInputFrom(IBuildingCanOutputItem building, Vector2Int inputPos)
    {
        var relevantPos = GetRelevantPos(inputPos);

        var id = GetInputPortIdFromRelPos(relevantPos);
        if (id == -1)
            return false;

        var factoryInfo = info as FactoryInfo;
        if (factoryInfo == null)
            return false;
        if (!factoryInfo.enableInputPorts.ElementAtOrDefault(id))
            return false;
        return inputBuildings[id] == null || building == null;
    }

    public IBuildingCanOutputItem[] GetInputBuildings()
    {
        return inputBuildings.Where((building) => building != null).ToArray();
    }

    public IBuildingCanInputItem[] GetOutputBuildings()
    {
        if (outputBuilding == null)
            return new IBuildingCanInputItem[] { };
        return new[] { outputBuilding };
    }

    public int GetCurrentRecipeId()
    {
        var factoryInfo = info as FactoryInfo;
        if (factoryInfo == null)
            return -1;

        var recipes = factoryInfo.recipes;

        for (var i = 0; i < recipes.Length; ++i)
            if (recipes[i] == CurrentRecipe)
                return i;
        return -1;
    }

    public override void Save(BinaryWriter writer)
    {
        // 配方
        SaveHelper.Write(writer, CurrentRecipe);

        // 缓存
        writer.Write(_inputItemCache?.Length ?? 0);
        if (_inputItemCache != null)
            foreach (var itemStack in _inputItemCache)
                SaveHelper.Write(writer, itemStack);

        writer.Write(_outputItemCache?.Length ?? 0);
        if (_outputItemCache != null)
            foreach (var itemStack in _outputItemCache)
                SaveHelper.Write(writer, itemStack);

        // 合成进度
        writer.Write(processingTime);

        //输入输出
        for (var i = 0; i < inputBuildings.Length; ++i)
            SaveHelper.Write(writer, inputBuildings[i]);
        SaveHelper.Write(writer, outputBuilding);
    }

    public override void Load(BinaryReader reader)
    {
        CurrentRecipe = SaveHelper.ReadScriptable<RecipeInfo>(reader);

        var inputCacheCount = reader.ReadInt32();
        _inputItemCache = new ItemStack[inputCacheCount];
        for (var i = 0; i < inputCacheCount; ++i)
            _inputItemCache[i] = SaveHelper.ReadItemStack(reader);

        var outputCacheCount = reader.ReadInt32();
        _outputItemCache = new ItemStack[outputCacheCount];
        for (var i = 0; i < outputCacheCount; ++i)
        {
            _outputItemCache[i] = SaveHelper.ReadItemStack(reader);

            if (_outputItemCache[i].count > 0)
                isAllItemPoped = false;
        }

        processingTime = reader.ReadSingle();

        for (var i = 0; i < inputBuildings.Length; ++i)
            inputBuildings[i] = SaveHelper.ReadBuildingCanOutput(reader);
        outputBuilding = SaveHelper.ReadBuildingCanInput(reader);
    }

    public void Reconnect()
    {
        var pos = _gridElement.CellPos;

        // 寻找输入
        BuildingBase foundBuilding = null;
        foreach (var inputPos in InputPos)
        {
            var id = GetInputPortIdFromRelPos(inputPos);
            foundBuilding = GameMap.GlobalMap.Get().GetBuildingAt(pos + inputPos);
            if (foundBuilding is IBuildingCanOutputItem foundOutputBuilding)
            {
                // 尝试连接
                Vector2Int connectDirection;
                if (inputPos.x < 0)
                    connectDirection = Vector2Int.right;
                else if (inputPos.y < 0)
                    connectDirection = Vector2Int.up;
                else if (inputPos.y > 2)
                    connectDirection = Vector2Int.down;
                else
                    connectDirection = Vector2Int.zero;
                if (foundOutputBuilding.TrySetOutputTo(this, pos + inputPos + connectDirection))
                    inputBuildings[id] = foundOutputBuilding;
            }
            else
                inputBuildings[id] = null;
        }

        // 寻找输出
        foundBuilding = GameMap.GlobalMap.Get().GetBuildingAt(pos + OutputPos);
        if (foundBuilding is IBuildingCanInputItem foundInputBuilding)
        {
            // 尝试连接
            if (foundInputBuilding.TrySetInputFrom(this, pos + OutputPos + Vector2Int.left))
                outputBuilding = foundInputBuilding;
        }
        else
            outputBuilding = null;
    }
}
