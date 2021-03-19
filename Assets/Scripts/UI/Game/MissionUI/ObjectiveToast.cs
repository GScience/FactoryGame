using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectiveToast : MonoBehaviour, IPointerClickHandler
{
    [Header("References")]
    [Tooltip("标题文字")]
    public TMPro.TextMeshProUGUI titleTextContent;
    [Tooltip("详情描述文字")]
    public TMPro.TextMeshProUGUI descriptionTextContent;
    [Tooltip("计数器文字")]
    public TMPro.TextMeshProUGUI counterTextContent;

    [Tooltip("呈现详情描述的矩形")]
    public RectTransform subTitleRect;
    [Tooltip("淡入/淡出的幕布")]
    public CanvasGroup canvasGroup;

    [Tooltip("包含对象的Layout group")]
    public HorizontalOrVerticalLayoutGroup layoutGroup;

    [Header("Transitions")]
    [Tooltip("移动完成前的延时")]
    public float completionDelay;
    [Tooltip("淡入的时间间隔")]
    public float fadeInDuration = 0.5f;
    [Tooltip("淡出的时间间隔")]
    public float fadeOutDuration = 2f;

    [Header("Sound")]
    [Tooltip("初始化声音")]
    public AudioClip initSound;
    [Tooltip("玩家完成声音")]
    public AudioClip completedSound;

    [Header("Movement")]
    [Tooltip("进入屏幕的时间")]
    public float moveInDuration = 0.5f;
    [Tooltip("进入的动画曲线, 随时间在x轴上发生位置变化")]
    public AnimationCurve moveInCurve;

    [Tooltip("退出屏幕的时间")]
    public float moveOutDuration = 2f;
    [Tooltip("退出的动画曲线, 随时间在x轴上发生位置变化")]
    public AnimationCurve moveOutCurve;

    public float Delay { get; set; }
    public bool CanFadeOut { get; set; }

    private static List<ObjectiveToast> _toastList = new List<ObjectiveToast>();

    public void UpdateCounterText(string text)
    {
        counterTextContent.text = text;
    }

    public void Initialize(string titleText, string descText, string counterText, float delay, bool canFadeOut)
    {
        // 设置对象描述，并更新Canvas
        Canvas.ForceUpdateCanvases();

        titleTextContent.text = titleText;
        descriptionTextContent.text = descText;
        counterTextContent.text = counterText;

        UpdatePos();

        Delay = delay;
        CanFadeOut = canFadeOut;

        if (CanFadeOut)
        {
            var insertIndex = _toastList.FindIndex((toast) => toast.CanFadeOut);
            if (insertIndex == -1)
                insertIndex = _toastList.Count;
            _toastList.Insert(insertIndex, this);
        }
        else
            _toastList.Insert(0, this);

        StartCoroutine(AnimCotourine());
    }

    void OnDestroy()
    {
        _toastList.Remove(this);
    }

    private void Update()
    {
        UpdatePos();
    }

    public void UpdatePos()
    {
        var id = _toastList.IndexOf(this);

        layoutGroup.padding.top = id * 100;
        layoutGroup.padding.bottom = id * -100;

        if (GetComponent<RectTransform>())
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    IEnumerator AnimCotourine()
    {
        // 淡入
        var totalFadeInTime = 0f;

        while (totalFadeInTime < fadeInDuration)
        {
            totalFadeInTime += Time.deltaTime;
            layoutGroup.padding.left = (int) moveInCurve.Evaluate(totalFadeInTime / fadeInDuration);
            canvasGroup.alpha = totalFadeInTime / fadeInDuration;
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            yield return 0;
        }

        // 延迟
        var totalWaitTime = 0f;

        while (
            _toastList.FindIndex((toast) => toast == this) < 3 && 
            totalWaitTime < Delay)
        {
            totalWaitTime += Time.deltaTime;
            yield return 0;
        }

        // 淡出
        var totalFadeOutTime = 0f;

        while (!CanFadeOut)
            yield return 0;

        while (totalFadeOutTime < fadeOutDuration)
        {
            totalFadeOutTime += Time.deltaTime;
            layoutGroup.padding.left = (int)moveOutCurve.Evaluate(totalFadeOutTime / fadeOutDuration);
            canvasGroup.alpha = 1 - totalFadeOutTime / fadeOutDuration;
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            yield return 0;
        }

        Destroy(gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Delay = 0;
    }
}
