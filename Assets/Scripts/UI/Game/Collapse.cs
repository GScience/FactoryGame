using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 支持隐藏和显示动画的UI
/// </summary>
[RequireComponent(typeof(Animator))]
public class Collapse : MonoBehaviour
{
    public enum AnimState
    {
        Open, Close, Opened, Closed
    }

    public UnityAction OnShown;
    public UnityAction OnHidden;

    public bool IsLocked = false;

    public AudioClip openAudioClip;
    public AudioClip closeAudioClip;

    private AudioSource _audioSource;

    [HideInInspector]
    public AnimState animState = AnimState.Closed; 

    /// <summary>
    /// 显示动画用的Animator
    /// </summary>
    private Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _animator.Play("Closed");

        if (openAudioClip != null || closeAudioClip != null)
            _audioSource =gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
            animState = AnimState.Open;
        else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Close"))
            animState = AnimState.Close;
        else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Opened"))
        {
            if (animState == AnimState.Open)
                OnShown?.Invoke();
            animState = AnimState.Opened;
        }
        else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Closed"))
        {
            if (animState == AnimState.Close)
                OnHidden?.Invoke();
            animState = AnimState.Closed;
        }
    }

    /// <summary>
    /// 切换显示状态，给按钮用的
    /// </summary>
    public void SwitchState()
    {
        if (IsLocked)
            return;

        if (animState == AnimState.Opened || animState == AnimState.Open)
            Close();
        else if (animState == AnimState.Closed || animState == AnimState.Close)
            Open();
    }

    public void Open()
    {
        if (_audioSource != null)
        {
            _audioSource.clip = openAudioClip;
            _audioSource.Play();
        }
        _animator.Play("Open");
    }

    public void Close()
    {
        if (_audioSource != null)
        {
            _audioSource.clip = closeAudioClip;
            _audioSource.Play();
        }
        _animator.Play("Close");
    }
}
