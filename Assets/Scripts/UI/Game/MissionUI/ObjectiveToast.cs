using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveToast : MonoBehaviour
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

    private float _delay;

    private static int ToastCount = 0;
    private int currentToastId = 0;

    public void Initialize(string titleText, string descText, string counterText, float delay)
    {
        // 设置对象描述，并更新Canvas
        Canvas.ForceUpdateCanvases();

        titleTextContent.text = titleText;
        descriptionTextContent.text = descText;
        counterTextContent.text = counterText;

        UpdatePos();

        _delay = delay;
        StartCoroutine(AnimCotourine());
    }

    private void Awake()
    {
        ToastCount++;
        currentToastId = ToastCount;
    }

    private void Update()
    {
        UpdatePos();
    }

    public void UpdatePos()
    {
        layoutGroup.padding.top = (ToastCount - currentToastId) * 100;
        layoutGroup.padding.bottom = (ToastCount - currentToastId) * -100;

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

        while (ToastCount - currentToastId < 5 && totalWaitTime < _delay)
        {
            totalWaitTime += Time.deltaTime;
            yield return 0;
        }

        // 淡出
        var totalFadeOutTime = 0f;

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
}
