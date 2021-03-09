using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RecipeUI : MonoBehaviour
{
    public ItemRequirementUI[] itemRequirementUIs;
    public ItemStackUI[] itemOutputUIs;

    private RecipeInfo currentRecipe;

    public void SetRecipe(RecipeInfo recipe)
    {
        if (currentRecipe == recipe)
            return;

        currentRecipe = recipe;

        if (recipe != null && 
            (recipe.input.Count > itemRequirementUIs.Length ||
            recipe.output.Count > itemOutputUIs.Length))
            Debug.LogError("Such a large recipe");

        for (var i = 0; i < itemRequirementUIs.Length; ++i)
        {
            // 为空的话清空
            if (recipe == null || i >= recipe.input.Count)
            {
                itemRequirementUIs[i].gameObject.SetActive(false);
                continue;
            }

            var requirement = recipe.input[i];
            itemRequirementUIs[i].gameObject.SetActive(true);
            itemRequirementUIs[i].needItem = requirement.item;
            itemRequirementUIs[i].requireCount = requirement.count;
        }

        for (var i = 0; i < itemOutputUIs.Length; ++i)
        {
            if (recipe == null || i >= recipe.output.Count)
            {
                itemOutputUIs[i].gameObject.SetActive(false);
                continue;
            }
            itemOutputUIs[i].gameObject.SetActive(true);
            itemOutputUIs[i].ItemStack = recipe.output[i];
        }
    }

    public void SetItemCount(int id, int count)
    {
        itemRequirementUIs[id].haveCount = count;
    }

    public void OnDisable()
    {
        SetRecipe(null);
    }
}
