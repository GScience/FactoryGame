using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 工厂基础UI
/// </summary>
[RequireComponent(typeof(Bubble))]
public class FactoryUI : MonoBehaviour
{
    public RecipeUI recipeUI;
    public ProgressBar progressBar;

    [HideInInspector]
    [NonSerialized]
    public Factory factory;

    void Update()
    {
        if (recipeUI == null)
            return;

        if (factory == null || factory.CurrentRecipe == null)
        {
            progressBar.enabled = false;
            recipeUI.enabled = false;
            return;
        }
        progressBar.enabled = true;
        recipeUI.enabled = true;

        recipeUI.SetRecipe(factory.CurrentRecipe);

        for (var i = 0; i < factory.CurrentRecipe.input.Count; ++i)
            recipeUI.SetItemCount(i, factory.GetInputCacheCount(i));

        progressBar.Percentage = factory.GetProgressPercentage();
    }
}
