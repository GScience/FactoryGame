using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 语言文件
/// </summary>
[CreateAssetMenu(fileName = "New Lang", menuName = "Game/Lang")]
public class Lang : ScriptableObject
{
    /// <summary>
    /// 传送带缺少连接
    /// </summary>
    public string Belt_Warning_No_Connection;

    /// <summary>
    /// 一切正常
    /// </summary>
    public string Building_Everything_Fine;

    /// <summary>
    /// 钱不够了
    /// </summary>
    public string Money_Not_Enough;
}
