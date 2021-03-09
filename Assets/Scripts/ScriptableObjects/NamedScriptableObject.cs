using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 带有hash的Scirptable object
/// </summary>
public class NamedScriptableObject : ScriptableObject
{
    /// <summary>
    /// 名称的hash值
    /// 用来在资源管理器中查找资源
    /// </summary>
    public int NameHash { get; set; }
}
