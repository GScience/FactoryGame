using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 工厂弹出菜单
/// </summary>
[RequireComponent(typeof(PopMenu))]
public class FactoryPopMenu : MonoBehaviour
{
    private PopMenu _popMenu;

    public ToggleGroup toggleGroup;

    /// <summary>
    /// 工厂
    /// </summary>
    [HideInInspector]
    [NonSerialized]
    public Factory factory;

    /// <summary>
    /// 布局组
    /// </summary>
    public Transform layoutGroup;

    /// <summary>
    /// 弹出元素预制体
    /// </summary>
    public FactoryPopMenuElement popMenuElementPrefab;

    /// <summary>
    /// 所有选择框
    /// </summary>
    private List<Toggle> _toggles = new List<Toggle>();

    private int _currentRecipeId = -1;

    public int CurrentRecipeId
    {
        get => _currentRecipeId;
        set
        {
            if (_currentRecipeId == value)
                return;
            _currentRecipeId = value;
            factory.SetCurrentRecipe(value);
        }
    }

    private void Awake()
    {
        _popMenu = GetComponent<PopMenu>();
    }

    private void Start()
    {
        // 设置工厂信息
        var factoryInfo = factory.info as FactoryInfo;
        if (factoryInfo == null)
            return;
        var recipes = factoryInfo.recipes;

        for (var i = 0; i < recipes.Length; ++i)
        {
            var recipe = recipes[i];
            var index = i;
            var element = Instantiate(popMenuElementPrefab, layoutGroup);
            element.factoryPopMenu = this;
            element.recipe = recipe;

            _toggles.Add(element.toggle);

            element.toggle.onValueChanged.AddListener((value) =>
            {
                if (value)
                    CurrentRecipeId = index;
            });
        }

        _currentRecipeId = factory.GetCurrentRecipeId();

        if (_currentRecipeId != -1)
            _toggles[_currentRecipeId].isOn = true;
    }

    private void Update()
    {
        if (!toggleGroup.AnyTogglesOn())
        {
            CurrentRecipeId = -1;
            return;
        }
    }
}
