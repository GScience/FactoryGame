using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 选择建造啥的选择器的逻辑代码
/// </summary>
[RequireComponent(typeof(Collapse))]
public class BuildingElementSelector : MonoBehaviour
{
    public static InstanceHelper<BuildingElementSelector> GlobalSelector;

    private Collapse _collapse;

    void Awake()
    {
        _collapse = GetComponent<Collapse>();
        GlobalSelector = new InstanceHelper<BuildingElementSelector>(this);
    }

    void Start()
    {
        _collapse.Open();
    }

    /// <summary>
    /// 关闭建筑选择器，自动锁定无法按钮打开
    /// </summary>
    public void Close()
    {
        _collapse.IsLocked = true;
        _collapse.Close();
    }

    /// <summary>
    /// 开启建筑选择器
    /// </summary>
    public void Open()
    {
        _collapse.IsLocked = false;
        _collapse.Open();
    }
}
