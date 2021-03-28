using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 处理环境音效
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class GameSound : MonoBehaviour
{
    /// <summary>
    /// 随机音效
    /// </summary>
    public AudioClip[] randomClips;

    /// <summary>
    /// 背景音乐音效
    /// </summary>
    public AudioClip[] bgmClips;

    public AudioSource backgroundSource;

    private AudioSource _environmentSource;

    private float _lastUpdateTime;

    private void Awake()
    {
        _environmentSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        var currentTime = Time.time;

        // 每一秒检查一次
        if (currentTime - _lastUpdateTime < 10)
            return;

        _lastUpdateTime = currentTime;

        // 随机重播背景音乐
        if (!backgroundSource.isPlaying)
        {
            var playRand = UnityEngine.Random.Range(0, 10);
            if (playRand < 2)
            {
                var idRand = UnityEngine.Random.Range(0, bgmClips.Length);
                backgroundSource.clip = bgmClips[idRand];
                backgroundSource.Play();
            }
        }

        // 随机播放环境音效
        if (!_environmentSource.isPlaying)
        {
            var playRand = UnityEngine.Random.Range(0, 50);
            if (playRand < 2)
            {
                var idRand = UnityEngine.Random.Range(0, randomClips.Length);
                _environmentSource.clip = randomClips[idRand];
                _environmentSource.Play();
            }
        }
    }
}
