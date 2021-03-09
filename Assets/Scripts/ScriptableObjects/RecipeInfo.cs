using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 合成配方
/// </summary>
[CreateAssetMenu(fileName = "New Recipe", menuName = "Game/Recipe")]
public class RecipeInfo : NamedScriptableObject
{
    /// <summary>
    /// 合成需求
    /// </summary>
    public List<ItemStack> input;

    /// <summary>
    /// 合成输出
    /// </summary>
    public List<ItemStack> output;

    /// <summary>
    /// 合成时间
    /// </summary>
    public float time;
}
