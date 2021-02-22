using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 工厂弹窗UI元素
/// </summary>
public class FactoryPopMenuElement : MonoBehaviour
{
    public Toggle toggle;
    public RecipeUI recipeUI;

    [HideInInspector]
    [NonSerialized]
    public FactoryPopMenu factoryPopMenu;

    [HideInInspector]
    [NonSerialized]
    public RecipeInfo recipe;

    private void Start()
    {
        toggle.group = factoryPopMenu.toggleGroup;
        recipeUI.SetRecipe(recipe);
    }
}
