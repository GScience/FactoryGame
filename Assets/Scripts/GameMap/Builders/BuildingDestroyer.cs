using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BuildingDestroyer : MonoBehaviour
{
    public static InstanceHelper<BuildingDestroyer> GlobalBuilder;

    public GridRenderer gridRenderer;
    public Grid grid;

    private bool _isOpen;
    private BuildingBase _selectedBuilding;

    public BuildingGuideBlock buildingGuideBlock;
    private List<BuildingGuideBlock> _guideBlocks = new List<BuildingGuideBlock>();
    private Action _onFinished;

    public void Open(Action onFinished)
    {
        gridRenderer.OnSelected();
        _isOpen = true;
        _onFinished = onFinished;
    }

    private void Awake()
    {
        GlobalBuilder = new InstanceHelper<BuildingDestroyer>(this);
    }

    void Update()
    {
        if (!_isOpen)
            return;

        // 获取鼠标所在的建筑
        var mousePos = PlayerInput.GetMousePosInWorld();
        var cellPos = grid.WorldToCell(mousePos);
        var building = GameMap.GlobalMap.Get().GetBuildingAt((Vector2Int)cellPos);

        // 刷新引导方块
        if (_selectedBuilding != building)
        {
            _selectedBuilding = building;
            UpdateGuideBlock();
        }

        // 是否移除建筑
        if (PlayerInput.GetMouseClick(0) && _selectedBuilding != null)
        {
            GameMap.GlobalMap.Get().DestroyBuilding(_selectedBuilding);
            _selectedBuilding = null;
            UpdateGuideBlock();
        }

        // 是否结束
        if (Input.GetKeyDown(KeyCode.Escape))
            Close();
    }

    public void Close()
    {
        _selectedBuilding = null;
        UpdateGuideBlock();
        _isOpen = false;
        _onFinished?.Invoke();
    }

    void UpdateGuideBlock()
    {
        var gridElement = _selectedBuilding?.GetComponent<GridElement>();
        var size = Vector2Int.zero;
        if (gridElement != null)
            size = gridElement.Size;
        var blockCount = size.x * size.y;

        while (_guideBlocks.Count < blockCount)
        {
            var newGuideBlock = Instantiate(buildingGuideBlock);
            _guideBlocks.Add(newGuideBlock);
        }

        while (_guideBlocks.Count > blockCount)
        {
            var oldGuideBlock = _guideBlocks[_guideBlocks.Count - 1];
            _guideBlocks.RemoveAt(_guideBlocks.Count - 1);
            Destroy(oldGuideBlock.gameObject);
        }

        var index = 0;
        for (var x = 0; x < size.x; ++x)
            for (var y = 0; y < size.y; ++y)
            {
                var guideBlock = _guideBlocks[index++];
                var pos = new Vector2(x + gridElement.CellPos.x + 0.5f, y + gridElement.CellPos.y + 0.5f);
                guideBlock.transform.position = pos;
                guideBlock.SetCanPlace(false);
            }
    }
}
