using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bubble : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    public bool IsMouseInBubble { get; private set; } = true;

    public void OnPointerExit(PointerEventData eventData)
    {
        IsMouseInBubble = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsMouseInBubble = true;
    }
}
