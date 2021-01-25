using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 显示玩家是否可以建造的一层覆盖
/// </summary>
[RequireComponent(typeof(Camera))]
public class CanPlayerBuildLayerPostEffect : MonoBehaviour
{
    public Grid grid;
    private Camera _camera;

    public Material lineMaterial;

    void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    void OnPostRender()
    {
        var width = _camera.pixelWidth;
        var height = _camera.pixelHeight;

        var leftUp = _camera.ScreenToWorldPoint(new Vector2(0, height));
        var rightDown = _camera.ScreenToWorldPoint(new Vector3(width, 0));

        // 竖线
        var vBegin = Mathf.FloorToInt(leftUp.x / grid.cellSize.x) * grid.cellSize.x;
        for (var x = vBegin; x < rightDown.x; x += grid.cellSize.x)
            DrawLine(new Vector2(x, leftUp.y), new Vector2(x, rightDown.y), Color.green);

        // 横线
        var hBegin = Mathf.FloorToInt(rightDown.y / grid.cellSize.y) * grid.cellSize.y;
        for (var y = hBegin; y < leftUp.y; y += grid.cellSize.y)
            DrawLine(new Vector2(leftUp.x, y), new Vector2(rightDown.x, y), Color.green);
    }
    void DrawLine(Vector2 from, Vector2 to, Color color)
    {
        GL.Begin(GL.LINES);
        lineMaterial.SetPass(0);
        GL.Color(color);
        GL.Vertex3(from.x, from.y, 0);
        GL.Vertex3(to.x, to.y, 0);
        GL.End();
    }

}
