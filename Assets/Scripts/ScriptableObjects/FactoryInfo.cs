using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "New Factory Info", menuName = "Game/Factory Info")]
public class FactoryInfo : BuildingInfo
{
    /// <summary>
    /// 合成配方
    /// </summary>
    public RecipeInfo[] recipes;

    public Vector2Int[] inputPos;
    public Vector2Int outputPos;
}
