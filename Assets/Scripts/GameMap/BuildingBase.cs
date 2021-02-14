using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
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
    private GridElement _gridElement;
    private Renderer _renderer;

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
}
