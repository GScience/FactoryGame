using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class Dragable : MonoBehaviour, IDragHandler
{
    public void OnDrag(PointerEventData eventData)
    {
        var rect = GetComponent<RectTransform>();
        rect.SetPositionAndRotation(rect.position + (Vector3)eventData.delta, rect.rotation);
    }
}
