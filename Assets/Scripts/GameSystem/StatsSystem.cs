using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 统计系统
/// </summary>
public class StatsSystem : ISystem
{
    /// <summary>
    /// 统计数据库
    /// 负责根据名称索引数据库
    /// </summary>
    public Dictionary<string, Dictionary<NamedScriptableObject, int>> statsDatabase = new Dictionary<string, Dictionary<NamedScriptableObject, int>>
    {
        { "BuildingCount",  new Dictionary<NamedScriptableObject, int>() },
        { "SellItemCount",  new Dictionary<NamedScriptableObject, int>() },
        { "ProcessItemCount",  new Dictionary<NamedScriptableObject, int>() },
        { "RecipeUseCount",  new Dictionary<NamedScriptableObject, int>() }
    };

    /// <summary>
    /// 建筑数量统计
    /// </summary>
    public Dictionary<NamedScriptableObject, int> BuildingCount => statsDatabase["BuildingCount"];

    /// <summary>
    /// 出售物品统计
    /// </summary>
    public Dictionary<NamedScriptableObject, int> SellItemCount => statsDatabase["SellItemCount"];

    /// <summary>
    /// 生产物品统计
    /// </summary>
    public Dictionary<NamedScriptableObject, int> ProcessItemCount => statsDatabase["ProcessItemCount"];

    /// <summary>
    /// 合成配方使用统计
    /// </summary>
    public Dictionary<NamedScriptableObject, int> RecipeUseCount => statsDatabase["RecipeUseCount"];

    /// <summary>
    /// 写入Dictionary
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="writer"></param>
    /// <param name=""></param>
    public static void ReadDictionary<TKey>(BinaryReader reader, Dictionary<TKey, int> dict) where TKey : NamedScriptableObject
    {
        int count = reader.ReadInt32();
        for (var i = 0; i < count; ++i)
        {
            var key = SaveHelper.ReadScriptable<TKey>(reader);
            var value = reader.ReadInt32();
            dict[key] = value;
        }
    }

    public void Load(BinaryReader reader)
    {
        var count = reader.ReadInt32();

        for (var i = 0; i < count; ++i)
        {
            var key = reader.ReadString();
            var statsCount = reader.ReadInt32();

            if (!statsDatabase.TryGetValue(key, out var dict))
                continue;

            for (var j = 0; j < statsCount; ++j)
            {
                var obj = SaveHelper.ReadScriptable<NamedScriptableObject>(reader);
                var value = reader.ReadInt32();
                dict.Add(obj, value);
            }
        }
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(statsDatabase.Count);

        foreach (var stats in statsDatabase)
        {
            var key = stats.Key;
            var dict = stats.Value;
            writer.Write(key);
            writer.Write(dict.Count);
            foreach (var pair in dict)
            {
                SaveHelper.Write(writer, pair.Key);
                writer.Write(pair.Value);
            }
        }
    }

    public void OnSellItem(ItemInfo item, int count)
    {
        if (!SellItemCount.ContainsKey(item))
            SellItemCount[item] = 1;
        else
            SellItemCount[item] = SellItemCount[item] + count;
    }

    public void OnProcessItem(ItemInfo item, int count)
    {
        if (!ProcessItemCount.ContainsKey(item))
            ProcessItemCount[item] = 1;
        else
            ProcessItemCount[item] = ProcessItemCount[item] + count;
    }

    public void OnBuildBuilding(BuildingInfo building)
    {
        if (!BuildingCount.ContainsKey(building))
            BuildingCount[building] = 1;
        else
            BuildingCount[building] = BuildingCount[building] + 1;
    }

    public void OnDestroyBuilding(BuildingInfo building)
    {
        BuildingCount[building] = BuildingCount[building] - 1;
    }

    public void OnSelectRecipe(RecipeInfo recipe)
    {
        if (!RecipeUseCount.ContainsKey(recipe))
            RecipeUseCount[recipe] = 1;
        else
            RecipeUseCount[recipe] = RecipeUseCount[recipe] + 1;
    }

    public void OnDeselectRecipe(RecipeInfo recipe)
    {
        RecipeUseCount[recipe] = RecipeUseCount[recipe] - 1;
    }

    public void Init()
    {

    }

    public void Update()
    {
        
    }
}
