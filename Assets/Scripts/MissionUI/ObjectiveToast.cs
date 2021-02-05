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
    [Tooltip("进入的动画曲线, 随时间在x轴上发生位置变化")]
    public AnimationCurve moveOutCurve;

    float m_StartFadeTime;
    bool m_IsFadingIn;
    bool m_IsFadingOut;
    bool m_IsMovingIn;
    bool m_IsMovingOut;
    AudioSource m_AudioSource;
    RectTransform m_RectTransform;

    public void Initialize(string titleText, string descText, string counterText, bool isOptional, float delay)
    {

        // 设置对象描述，并更新Canvas
        Canvas.ForceUpdateCanvases();

        titleTextContent.text = titleText;
        descriptionTextContent.text = descText;
        counterTextContent.text = counterText;

        if (GetComponent<RectTransform>())
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }

        // 设置fade in时间
        m_StartFadeTime = Time.time + delay;

        // 对象处于fading in状态
        m_IsFadingIn = true;
        m_IsMovingIn = true;

    }

    public void Complete()
    {
        // 设置fade out时间
        m_StartFadeTime = Time.time + completionDelay;
        m_IsFadingIn = false;
        m_IsMovingIn = false;

        // 如果有设置声音，使其播放
        PlaySound(completedSound);

        // 对象处于fading out状态
        m_IsFadingOut = true;
        m_IsMovingOut = true;
    }

    void PlaySound(AudioClip sound)
    {
        if (!sound)
            return;

        if (!m_AudioSource)
        {
            m_AudioSource = gameObject.AddComponent<AudioSource>();
            m_AudioSource.outputAudioMixerGroup = AudioUtility.GetAudioGroup(AudioUtility.AudioGroups.HUDObjective);
        }

        m_AudioSource.PlayOneShot(sound);
    }

    private void Update()
    {
        // 计算fade状态的时间
        float timeSinceFadeStarted = Time.time - m_StartFadeTime;

        subTitleRect.gameObject.SetActive(!string.IsNullOrEmpty(descriptionTextContent.text));

        if (m_IsFadingIn && !m_IsFadingOut)
        {
            // 渐入
            if (timeSinceFadeStarted < fadeInDuration)
            {
                // 计算透明度
                canvasGroup.alpha = timeSinceFadeStarted / fadeInDuration;
            }
            // 渐入结束
            else
            {
                canvasGroup.alpha = 1f;
                m_IsFadingIn = false;

                // 播放声音
                PlaySound(initSound);
            }
        }

        if (m_IsMovingIn && !m_IsMovingOut)
        {
            // 进入
            if (timeSinceFadeStarted < moveInDuration)
            {
                layoutGroup.padding.left = (int)moveInCurve.Evaluate(timeSinceFadeStarted / moveInDuration);

                if (GetComponent<RectTransform>())
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
                }
            }
            // 进入结束
            else
            {
                
                layoutGroup.padding.left = 0;

                if (GetComponent<RectTransform>())
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
                }

                m_IsMovingIn = false;
            }

        }

        if (m_IsFadingOut)
        {
            // 淡出
            if (timeSinceFadeStarted < fadeOutDuration)
            {
                // 计算透明度
                canvasGroup.alpha = 1 - (timeSinceFadeStarted) / fadeOutDuration;
            }
            // 淡出结束
            else
            {
                canvasGroup.alpha = 0f;
                m_IsFadingOut = false;

                // 清除对象
                Destroy(gameObject);
            }
        }

        if (m_IsMovingOut)
        {
            // 移出
            if (timeSinceFadeStarted < moveOutDuration)
            {
                layoutGroup.padding.left = (int)moveOutCurve.Evaluate(timeSinceFadeStarted / moveOutDuration);

                if (GetComponent<RectTransform>())
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
                }
            }
            // 移出结束
            else
            {
                m_IsMovingOut = false;
            }
        }
    }
}
