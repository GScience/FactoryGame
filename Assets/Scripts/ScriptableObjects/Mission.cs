using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 任务
/// </summary>
public abstract class Mission : NamedScriptableObject
{
    /// <summary>
    /// 检查是否满足条件
    /// </summary>
    /// <returns></returns>
    public abstract bool Check();

    /// <summary>
    /// 任务的提示信息
    /// </summary>
    public string desc;

    /// <summary>
    /// 任务提示的头衔
    /// </summary>
    public abstract string Prefix { get; }

    /// <summary>
    /// 任务状态字符串
    /// </summary>
    public abstract string MissionState { get; }

    /// <summary>
    /// 是否显示任务完成
    /// </summary>
    public abstract bool ShowMissionFinish { get; }

    /// <summary>
    /// 奖励
    /// </summary>
    public BuildingCardBase[] cards;

    /// <summary>
    /// 音效
    /// </summary>
    public AudioClip audio;
}
