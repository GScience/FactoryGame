using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 地图元件，相当于所有Building的通用逻辑代码
/// </summary>
[RequireComponent(typeof(Renderer), typeof(GridElement))]
public abstract class BuildingBase : MonoBehaviour
{
    /// <summary>
    /// 建筑ID，用来在地图上查找建筑
    /// </summary>
    [HideInInspector]
    [NonSerialized]
    public int id;

    // 建筑信息
    public BuildingInfo info;
    public UpdaterRef UpdaterRef;
    protected GridElement _gridElement;
    protected Renderer _renderer;

    /// <summary>
    /// 初始建筑的hash值
    /// 用来在资源管理器中查找资源
    /// </summary>
    public int NameHash { get; set; }

    /// <summary>
    /// 是否在预览模式
    /// </summary>
    public bool IsPreviewMode { get; private set; }

    void Awake()
    {
        _gridElement = GetComponent<GridElement>();
        _renderer = GetComponent<Renderer>();
    }

    public void EnterPreviewMode()
    {
        _gridElement.enabled = true;
        IsPreviewMode = true;
    }

    public void ExitPreviewMode()
    {
        _gridElement.enabled = false;
        IsPreviewMode = false;
    }

    public abstract void OnMouseEnter();
    public abstract void OnMouseLeave();

    public abstract void OnClick();

    /// <summary>
    /// 当被放在地图上
    /// </summary>
    public abstract void OnPlace();

    /// <summary>
    /// 切换到下一个状态
    /// </summary>
    public virtual void ChangeToState(int offset)
    {
    }

    public Vector2Int GetRelevantPos(Vector2Int pos)
    {
        return pos - _gridElement.CellPos;
    }

    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="writer"></param>
    public virtual void Save(BinaryWriter writer)
    {
    }

    /// <summary>
    /// 读取
    /// </summary>
    /// <param name="reader"></param>
    public virtual void Load(BinaryReader reader)
    {
    }
}
