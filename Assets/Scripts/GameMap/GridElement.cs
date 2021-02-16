using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 网格地图原件，吸附网格什么的
/// </summary>
[ExecuteAlways]
public class GridElement : MonoBehaviour
{
    private Grid _grid;
    private Renderer _renderer;

    [SerializeField]
    private Vector2Int _size;

    public Vector2Int Size
    {
        get => _size;
        set
        {
            _size = value;
            OnSizeChanged();
        }
    }

    private Vector2Int _cellPos = Vector2Int.zero;
    public Vector2Int CellPos
    {
        get => _cellPos;
        set
        {
            _cellPos = value;
#if UNITY_EDITOR
            if (_grid == null)
                return;
#endif
            var offset = new Vector3((Size.x - 1) * _grid.cellSize.x / 2, (Size.y - 1) * _grid.cellSize.y / 2, 0);
            var cellCenter = _grid.GetCellCenterWorld((Vector3Int)_cellPos);
            transform.position = cellCenter + offset;
        }
    }

    void Awake()
    {
        if (_grid == null)
            _grid = GetComponentInParent<Grid>();
        _renderer = GetComponent<Renderer>();
    }

    void Start()
    {
        OnSizeChanged();
    }

    void OnSizeChanged()
    {
#if UNITY_EDITOR
        if (_renderer == null)
            return;
#endif
        var rendererSize = _renderer.bounds.size;
        transform.localScale = new Vector3(
            transform.localScale.x / rendererSize.x * Size.x,
            transform.localScale.y / rendererSize.y * Size.y,
            transform.localScale.z);
    }

    void OnDisable()
    {
        LateUpdate();
    }

    void LateUpdate()
    {
#if UNITY_EDITOR
        if (_grid == null)
            return;
#endif
        var offset = new Vector3((Size.x - 1) * _grid.cellSize.x / 2, (Size.y - 1) * _grid.cellSize.y / 2, 0);
        _cellPos = (Vector2Int)_grid.WorldToCell(transform.position - offset);
        var cellCenter = _grid.GetCellCenterWorld((Vector3Int)_cellPos);
        transform.position = cellCenter + offset;

        _renderer.sortingOrder = -_cellPos.y;
    }

    /// <summary>
    /// 获取与当前GridElement所碰撞的元素
    /// </summary>
    /// <returns></returns>
    public GridElement GetCollidingElement()
    {
        var gameMap = GameMap.GlobalMap.Get();

        for (var x = 0; x < Size.x; ++x)
            for (var y = 0; y < Size.y; ++y)
            {
                var building = gameMap.GetBuildingAt(new Vector2Int(CellPos.x + x, CellPos.y + y));
                if (building == null)
                    continue;
                var gridElement = building.GetComponent<GridElement>();
                if (gridElement != null)
                    return gridElement;
            }

        return null;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GridElement))]
    class GridElementEditor : Editor
    {
        private GridElement _gridElement;

        void OnEnable()
        {
            _gridElement = serializedObject.targetObject as GridElement;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var newSize = EditorGUILayout.Vector2IntField("Size", _gridElement.Size);

            var newPos = Vector2Int.zero;
            if (_gridElement._grid != null)
                newPos = EditorGUILayout.Vector2IntField("Position", _gridElement.CellPos);
            
            if (newSize != _gridElement.Size)
            {
                if (newSize.x < 1)
                    newSize.x = 1;
                if (newSize.y < 1)
                    newSize.y = 1;
                _gridElement.Size = newSize;
                EditorUtility.SetDirty(_gridElement);
            }

            if (newPos != _gridElement.CellPos)
                _gridElement.CellPos = newPos;

            for (var x = 0; x < _gridElement.Size.x; ++x)
            {
                var line = "";
                for (var y = 0; y < _gridElement.Size.y; ++y)
                    line += "■";
                EditorGUILayout.LabelField(line);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}