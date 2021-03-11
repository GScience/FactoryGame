using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 弹出菜单UI层
/// </summary>
[RequireComponent(typeof(Canvas))]
public class PopMenuLayer : MonoBehaviour
{
    public static InstanceHelper<PopMenuLayer> GlobalPopMenuLayer;

    /// <summary>
    /// 所有可使用的弹出菜单
    /// </summary>
    public PopMenu[] popMenus;

    /// <summary>
    /// 已经弹出的菜单
    /// </summary>
    private List<PopMenu> _popedMenus = new List<PopMenu>();

    private void Awake()
    {
        GlobalPopMenuLayer = new InstanceHelper<PopMenuLayer>(this);
    }

    public bool HasPopedMenu => _popedMenus.Count != 0;

    /// <summary>
    /// 尝试关闭但是已经空了的时候
    /// </summary>
    public Action TryCloseButEmpty;

    /// <summary>
    /// 弹出
    /// </summary>
    /// <param name="uiName"></param>
    /// <returns></returns>
    public PopMenu Pop(string uiName)
    {
        var prefab = popMenus.FirstOrDefault(menu => menu.name == uiName);

        var clone = Instantiate(prefab);
        clone.transform.SetParent(transform);

        _popedMenus.Add(clone);

        return clone;
    }

    public void Close(PopMenu menu)
    {
        // 查找到弹出窗口
        for (var i = 0; i < _popedMenus.Count; ++i)
        {
            if (_popedMenus[i] != menu)
                continue;

            // 移除以后的窗口
            for (var j = i; j < _popedMenus.Count; ++j)
                Destroy(_popedMenus[i].gameObject);
            _popedMenus.RemoveRange(i, _popedMenus.Count - i);
            break;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_popedMenus.Count > 0)
                Close(_popedMenus[_popedMenus.Count - 1]);
            else
                TryCloseButEmpty?.Invoke();
        }
    }
}
