using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 存储游戏最基本的信息
/// </summary>
[Serializable]
public struct GameInfo
{
    /// <summary>
    /// 游戏版本
    /// </summary>
    public int version;

    /// <summary>
    /// 存档名称
    /// </summary>
    public string name;

    /// <summary>
    /// 最后修改时间
    /// </summary>
    public string lastModifyTime;

    /// <summary>
    /// 创建时间
    /// </summary>
    public string createTime;
}
