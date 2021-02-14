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

    private void Awake()
    {
        GlobalBubbleUILayer = new InstanceHelper<BubbleUILayer>(this);
    }

    /// <summary>
    /// 弹出UI
    /// </summary>
    /// <param name="prefab"></param>
    public Bubble Pop(string name)
    {
        var prefab = bubbles.Where((bubble) => bubble.name == name).FirstOrDefault();

        var clone = Instantiate(prefab);
        clone.transform.SetParent(transform);
        var pos = new Vector2(
            Input.mousePosition.x,
            Input.mousePosition.y + clone.GetComponent<RectTransform>().sizeDelta.y / 2);
        clone.transform.position = pos;

        return clone;
    }

    public void Close(Bubble bubble)
    {
        Destroy(bubble.gameObject);
    }
}