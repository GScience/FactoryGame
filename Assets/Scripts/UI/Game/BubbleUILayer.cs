using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 气泡UI管理器
/// </summary>
[RequireComponent(typeof(Canvas))]
public class BubbleUILayer : MonoBehaviour
{
    public static InstanceHelper<BubbleUILayer> GlobalBubbleUILayer;

    /// <summary>
    /// 所有可使用的气泡
    /// </summary>
    public Bubble[] bubbles;

    private Canvas _canvas;

    private void Awake()
    {
        GlobalBubbleUILayer = new InstanceHelper<BubbleUILayer>(this);
        _canvas = GetComponent<Canvas>();
    }

    public float GetScale()
    {
        return _canvas.scaleFactor;
    }

    /// <summary>
    /// 弹出UI
    /// </summary>
    /// <param uiName="prefab"></param>
    public Bubble Pop(string uiName)
    {
        var prefab = bubbles.FirstOrDefault(bubble => bubble.name == uiName);

        var clone = Instantiate(prefab);
        clone.transform.SetParent(transform);
        clone.transform.localScale = Vector3.one;

        return clone;
    }

    public void Close(Bubble bubble)
    {
        Destroy(bubble.gameObject);
    }
}