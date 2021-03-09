using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class Bubble : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    public const float FadeInTime = 0.01f;

    public bool IsMouseInBubble { get; private set; } = true;

    private RectTransform _rect;
    private CanvasGroup _canvasGroup;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsMouseInBubble = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsMouseInBubble = true;
    }

    void Start()
    {
        UpdatePos();
        StartCoroutine(FadeIn());
    }

    void Update()
    {
        UpdatePos();
    }

    IEnumerator FadeIn()
    {
        for (var time = 0f; time < FadeInTime; time += Time.deltaTime)
        {
            _canvasGroup.alpha = time / FadeInTime;
            yield return 0;
        }

        _canvasGroup.alpha = 1;
    }

    public void UpdatePos()
    {
        transform.position = new Vector3(
            Input.mousePosition.x + _rect.sizeDelta.x / 2,
            Input.mousePosition.y + _rect.sizeDelta.y / 2);
    }
}
