using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 弹出窗口
/// </summary>
public class PopMenu : MonoBehaviour
{
    /// <summary>
    /// 关闭当前菜单
    /// </summary>
    public void Close()
    {
        PopMenuLayer.GlobalPopMenuLayer.Get().Close(this);
    }

    private void Start()
    {
        var rect = GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;
    }
}
