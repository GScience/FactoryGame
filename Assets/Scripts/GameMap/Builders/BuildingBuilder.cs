using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 工厂建造器
/// 移动、建造建筑时让建筑在鼠标位置
/// </summary>
public class BuildingBuilder : MonoBehaviour
{
    public static InstanceHelper<BuildingBuilder> GlobalBuilder;

    /// <summary>
    /// 当前所选择的建筑
    /// </summary>
    private BuildingBase _pickedBuilding;

    private Action _onConfirm;
    private Action _onCancel;

    public GridRenderer gridRenderer;

    /// <summary>
    /// 用于显示是否能建造的小方块
    /// </summary>
    public BuildingGuideBlock buildingGuideBlock;

    /// <summary>
    /// 用于显示是否能建造
    /// </summary>
    private List<BuildingGuideBlock> _buildingGuidingBlocks = new List<BuildingGuideBlock>();

    /// <summary>
    /// 是否正在建造
    /// </summary>
    public bool IsBuilding => _pickedBuilding != null;

    /// <summary>
    /// 上一次的位置
    /// </summary>
    private Vector2Int _lastCellPos = new Vector2Int(int.MaxValue, int.MaxValue);

    void Awake()
    {
        GlobalBuilder = new InstanceHelper<BuildingBuilder>(this);
    }

    void Update()
    {
        if (_pickedBuilding == null)
            return;

        // 锁定位置
        transform.position = _pickedBuilding.transform.position;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        // 刷新Guide Block
        UpdateGuidingBlock();

        // 按住鼠标的时候不需要移动
        if (!Input.GetMouseButton(0))
            _pickedBuilding.transform.position = PlayerInput.GetMousePosInWorld();

        if (PlayerInput.GetMouseClick(0))
        {
            if (CanBuild(_pickedBuilding.GetComponent<GridElement>()))
                OnConfirm();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
            OnCancel();
        else
        {
            // 滚动旋转
            if (Input.mouseScrollDelta.y > float.Epsilon)
                _pickedBuilding.ChangeToState(1);
            else if (Input.mouseScrollDelta.y < -float.Epsilon)
                _pickedBuilding.ChangeToState(-1);

        }
    }

    /// <summary>
    /// 检测是否能建造
    /// </summary>
    /// <param name="gridElement"></param>
    /// <returns></returns>
    private static bool CanBuild(GridElement gridElement)
    {
        var collision = gridElement.GetCollidingElement();
        return collision == null;
    }

    public void Pick(BuildingBase obj, Action onConfirm, Action onCancel)
    {
        if (_pickedBuilding != null)
            OnCancel();

        _pickedBuilding = obj;
        _onConfirm = onConfirm;
        _onCancel = onCancel;

        _pickedBuilding.transform.position = PlayerInput.GetMousePosInWorld();

        BuildingInformationBoard.GlobalBuildingInformationBoard.Get().ShowInformation(_pickedBuilding);
        gridRenderer.OnSelected();

        GenerateGuidingBlock();
    }

    /// <summary>
    /// 生成引导小方块
    /// </summary>
    void GenerateGuidingBlock()
    {
        var gridElement = _pickedBuilding.GetComponent<GridElement>();
        for (var x = 0; x < gridElement.Size.x; ++x)
        for (var y = 0; y < gridElement.Size.y; ++y)
        {
            var guideBlock = Instantiate(buildingGuideBlock);
            _buildingGuidingBlocks.Add(guideBlock);
            guideBlock.transform.SetParent(transform);
            guideBlock.transform.localPosition = new Vector3(x - gridElement.Size.x / 2.0f + 0.5f, y - gridElement.Size.y / 2.0f + 0.5f);
        }
    }

    /// <summary>
    /// 刷新引导小方块
    /// </summary>
    void UpdateGuidingBlock()
    {
        var gridElement = _pickedBuilding.GetComponent<GridElement>();

        if (_lastCellPos == gridElement.CellPos)
            return;

        var index = 0;
        for (var x = 0; x < gridElement.Size.x; ++x)
            for (var y = 0; y < gridElement.Size.y; ++y)
            {
                var guideBlock = _buildingGuidingBlocks[index++];
                var pos = new Vector2Int(x + gridElement.CellPos.x, y + gridElement.CellPos.y);
                guideBlock.SetCanPlace(GameMap.GlobalMap.Get().GetBuildingAt(pos) == null);
            }


        _lastCellPos = gridElement.CellPos;
    }

    void OnConfirm()
    {
        if (_pickedBuilding == null)
            return;
        var cost = _pickedBuilding.info.cost;
        if (!GameManager.MoneySystem.RequireMoney(cost))
        {
            var msgBox = PopMenuLayer.GlobalPopMenuLayer.Get().Pop("SimpleMessageBox").GetComponent<SimpleMessageBox>();
            msgBox.message = LangManager.Current.Money_Not_Enough.Replace("[BUILDING]", _pickedBuilding.info.buildingName);
            return;
        }
        _onConfirm?.Invoke();
        GameMap.GlobalMap.Get().PutBuildingOnMap(_pickedBuilding);
        OnFinished();
    }

    void OnCancel()
    {
        if (_pickedBuilding == null)
            return;
        _onCancel?.Invoke();
        OnFinished();
    }

    void OnFinished()
    {
        foreach (var sprite in _buildingGuidingBlocks)
            Destroy(sprite.gameObject);
        _buildingGuidingBlocks.Clear();

        _pickedBuilding = null;
        BuildingInformationBoard.GlobalBuildingInformationBoard.Get().HideInformation();
        gridRenderer.OnUnselected();

        _lastCellPos = new Vector2Int(int.MaxValue, int.MaxValue);
    }
}
