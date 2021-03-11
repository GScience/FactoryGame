using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

/// <summary>
/// 时间系统
/// </summary>
public class TimeSystem : ISystem
{
    /// <summary>
    /// 总时间
    /// </summary>
    public float TotalTime { get; private set; } = 0;

    /// <summary>
    /// 周期
    /// </summary>
    public int Week { get => ((int)TotalTime / 168); }

    /// <summary>
    /// 天
    /// </summary>
    public int Day { get => ((int)TotalTime / 24) - Week * 7; }

    /// <summary>
    /// 小时
    /// </summary>
    public int Hour { get => ((int)TotalTime) - Week * 168 - Day * 24; }

    public void Update()
    {
        TotalTime += Time.deltaTime * 1f;
    }

    public override string ToString()
    {
        return Week + "周 " + Day + "天 " + Hour + "小时";
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(TotalTime);
    }

    public void Load(BinaryReader reader)
    {
        TotalTime = reader.ReadSingle();
    }
}
