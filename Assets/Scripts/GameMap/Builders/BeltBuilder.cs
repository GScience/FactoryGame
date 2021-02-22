using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class BeltBuilder : MonoBehaviour
{
    public static InstanceHelper<BeltBuilder> GlobalBuilder;
    public GridRenderer gridRenderer;
    public BuildingGuideBlock BuildingGuideBlock;

    private Belt _straight;
    private Belt _cw;
    private Belt _ccw;

    private Action _onFinished;

    private readonly List<(Belt belt, BuildingGuideBlock guideBlock)> _previewsBelts = new List<(Belt belt, BuildingGuideBlock guideBlock)>();

    private Vector2Int _lastCellPos = new Vector2Int(int.MaxValue, int.MaxValue);

    private Vector2Int _lastDirection;
    private int _lastLength;

    /// <summary>
    /// 末端连接的建筑
    /// </summary>
    private IBuildingCanInputItem _endpointBuilding;

    /// <summary>
    /// 开始端连接的建筑
    /// </summary>
    private IBuildingCanOutputToOther _startpointBuilding;

    /// <summary>
    /// 是否选择了起始地点
    /// </summary>
    private bool _hasChosenStartPos;

    /// <summary>
    /// 是否正在建造
    /// </summary>
    public bool IsBuilding => _straight != null;

    void Awake()
    {
        GlobalBuilder = new InstanceHelper<BeltBuilder>(this);
    }

    public void ChooseBeltPrefabs(Belt straight, Belt cw, Belt ccw, Action onFinish)
    {
        BuildingInformationBoard.GlobalBuildingInformationBoard.Get().ShowInformation(straight);
        gridRenderer.OnSelected();
        _straight = straight;
        _cw = cw;
        _ccw = ccw;
        _onFinished = onFinish;

        GeneratePreviewBelt();

        _previewsBelts[0].belt.transform.position = PlayerInput.GetMousePosInWorld();
    }

    private (Belt belt, BuildingGuideBlock guideBlock) GeneratePreviewBelt()
    {
        var belt = GenerateBeltFromPrefab(_straight);
        var guideBlock = Instantiate(BuildingGuideBlock);
        guideBlock.transform.SetParent(transform);
        guideBlock.transform.localPosition = Vector3.zero;
        belt.transform.position = PlayerInput.GetMousePosInWorld();
        _previewsBelts.Add((belt, guideBlock));
        return (belt, guideBlock);
    }

    private Belt GenerateBeltFromPrefab(Belt prefab)
    {
        var belt = GameMap.GlobalMap.Get().CreateBuildingPreview(prefab) as Belt;
        return belt;
    }

    private bool TryRemoveTopPreviewBelt()
    {
        if (_previewsBelts.Count <= 1)
            return false;
        Destroy(_previewsBelts[_previewsBelts.Count - 1].belt.gameObject);
        Destroy(_previewsBelts[_previewsBelts.Count - 1].guideBlock.gameObject);
        _previewsBelts.RemoveAt(_previewsBelts.Count - 1);
        return true;
    }

    void Update()
    {
        if (_straight == null)
            return;

        transform.position = _previewsBelts[0].belt.transform.position;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        // 如果没选择起始地点，则等玩家点一下固定起点
        if (!_hasChosenStartPos)
            UpdateFirstSelection();
        else
            UpdateDragging();
    }

    // 刷新第一个选择
    private void UpdateFirstSelection()
    {       
        // 第一个传送带是起点
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnCancelled();
            return;
        }

        var beginBelt = _previewsBelts[0];

        // 移动预览建筑
        beginBelt.belt.transform.position = PlayerInput.GetMousePosInWorld();
        var gridElement = beginBelt.belt.GetComponent<GridElement>();

        if (_lastCellPos != gridElement.CellPos)
            _previewsBelts[0].guideBlock.SetCanPlace(gridElement.GetCollidingElement() == null);

        _lastCellPos = gridElement.CellPos;

        // 如果玩家点击了则代表选择了
        if (PlayerInput.GetMouseClick(0))
            _hasChosenStartPos = true;
    }

    // 刷新拖动
    private void UpdateDragging()
    {
        var beginBelt = _previewsBelts[0];

        // 计算传送带长度
        var startPos = (Vector2)beginBelt.belt.transform.position;
        var mousePos = PlayerInput.GetMousePosInWorld();
        var length = (int)(Vector2.Distance(startPos, mousePos) + 1.5f);

        // 计算方向
        var deltaPos = mousePos - startPos;
        Vector2Int direction;
        if (Mathf.Abs(deltaPos.x) > Mathf.Abs(deltaPos.y))
            direction = deltaPos.x > 0 ? Vector2Int.right : Vector2Int.left;
        else
            direction = deltaPos.y > 0 ? Vector2Int.up : Vector2Int.down;

        if (length != _lastLength || direction != _lastDirection)
        {
            // 产生满足数量的传送带
            while (length > _previewsBelts.Count)
                GeneratePreviewBelt();

            // 删除传送带
            while (length < _previewsBelts.Count && TryRemoveTopPreviewBelt()) { }

            // 计算传送带位置
            var directions = new[] {Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right};

            for (var i = 0; i < _previewsBelts.Count; ++i)
            {
                var offset = (Vector3Int) direction * i;
                _previewsBelts[i].belt.transform.position = beginBelt.belt.transform.position + offset;

                var belt = _previewsBelts[i].belt;
                var gridElement = belt.GetComponent<GridElement>();

                // 是否需要设置为直的
                var needResetToStraightFoundBuilding = true;

                // 首位两个传送带需要手动查找连接，可以是曲的，特殊处理
                if (i == 0 || i == _previewsBelts.Count - 1)
                {
                    IBuildingCanInputItem findBuilding = null;

                    foreach (var dir in directions)
                    {
                        if (i == 0)
                        {
                            var building = FindBuildingInDirection<IBuildingCanOutputToOther>(gridElement, dir);
                            if (building == null)
                                continue;
                            _startpointBuilding = building;
                        }
                        else
                        {
                            var building = FindBuildingInDirection<IBuildingCanInputItem>(gridElement, dir);
                            if (building == null)
                                continue;
                            findBuilding = building;
                        }

                        needResetToStraightFoundBuilding = false;

                        var inDirection = i == 0 ? -dir : direction;
                        var outDirection = i == 0 ? direction : dir;

                        // 获取传送带对应的类型
                        var beltType = CheckCornerBeltType(inDirection, outDirection);

                        if (beltType == _previewsBelts[i].belt.beltType)
                            continue;

                        // 创建传送带
                        Belt newBelt;
                        switch (beltType)
                        {
                            case Belt.BeltType.Straight:
                                newBelt = GenerateBeltFromPrefab(_straight);
                                break;
                            case Belt.BeltType.CornerCcw:
                                newBelt = GenerateBeltFromPrefab(_ccw);
                                break;
                            case Belt.BeltType.CornerCw:
                                newBelt = GenerateBeltFromPrefab(_cw);
                                break;
                            default:
                                throw new InvalidEnumArgumentException();
                        }

                        var pos = _previewsBelts[i].belt.transform.position;
                        Destroy(_previewsBelts[i].belt.gameObject);
                        newBelt.transform.position = pos;
                        _previewsBelts[i] = (newBelt, _previewsBelts[i].guideBlock);
                        _previewsBelts[i].belt.SetBeltDirection(inDirection);
                    }

                    if (i != 0)
                        _endpointBuilding = findBuilding;
                }

                // 其他位置全是直的
                if (_previewsBelts[i].belt.beltType != Belt.BeltType.Straight)
                {
                    if (needResetToStraightFoundBuilding)
                    {
                        var newBelt = GenerateBeltFromPrefab(_straight);
                        var pos = _previewsBelts[i].belt.transform.position;
                        Destroy(_previewsBelts[i].belt.gameObject);
                        newBelt.transform.position = pos;
                        _previewsBelts[i] = (newBelt, _previewsBelts[i].guideBlock);
                        _previewsBelts[i].belt.SetBeltDirection(direction);
                    }
                }
                else
                    _previewsBelts[i].belt.SetBeltDirection(direction);

                var guideBlock = _previewsBelts[i].guideBlock;
                var guideBlockTransform = guideBlock.transform;

                if (guideBlockTransform.localPosition != offset)
                {
                    guideBlockTransform.localPosition = offset;
                    var canPlace = gridElement.GetCollidingElement() == null;
                    guideBlock.SetCanPlace(canPlace);
                }
            }

            _lastDirection = direction;
            _lastLength = length;
        }

        // 开始尝试建造
        if (PlayerInput.GetMouseClick(0))
        {
            if (CanBuild())
                OnDraggingConfirm();
        }

        else if (Input.GetKeyDown(KeyCode.Escape))
            OnDraggingCancelled();
    }

    /// <summary>
    /// 根据方向来获取传送带
    /// </summary>
    /// <param name="inDirection"></param>
    /// <param name="outDirection"></param>
    /// <returns></returns>
    private Belt.BeltType CheckCornerBeltType(Vector2Int inDirection, Vector2 outDirection)
    {
        if (inDirection * outDirection != Vector2Int.zero)
            return Belt.BeltType.Straight;
        if (Vector2.SignedAngle(inDirection, outDirection) < 0)
            return Belt.BeltType.CornerCw;
        return Belt.BeltType.CornerCcw;
    }

    /// <summary>
    /// 在指定方向上找传建筑
    /// </summary>
    /// <param name="gridElement"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    private T FindBuildingInDirection<T>(GridElement gridElement, Vector2Int direction) where T : class
    {
        var gameMap = GameMap.GlobalMap.Get();
        var pos = gridElement.CellPos + direction;
        var building = gameMap.GetBuildingAt(pos);

        if (building is T value)
            return value;
        return null;
    }

    // 是否能建造
    private bool CanBuild()
    {
        foreach (var belt in _previewsBelts)
        {
            var gridElement = belt.belt.GetComponent<GridElement>().GetCollidingElement();
            if (gridElement != null)
                return false;
        }

        return true;
    }

    void OnFinish()
    {
        // 删除所有还在列表里的东西
        foreach (var (belt, guideBlock) in _previewsBelts)
        {
            Destroy(guideBlock.gameObject);
            Destroy(belt.gameObject);
        }

        _straight = null;
        _hasChosenStartPos = false;
        _endpointBuilding = null;
        _startpointBuilding = null;
        _previewsBelts.Clear();
        gridRenderer.OnUnselected();
        BuildingInformationBoard.GlobalBuildingInformationBoard.Get().HideInformation();
        _onFinished?.Invoke();
    }

    /// <summary>
    /// 拖动确认
    /// </summary>
    void OnDraggingConfirm()
    {
        var gameMap = GameMap.GlobalMap.Get();

        // 最后一个传送带还是预览
        var continueDragging = _endpointBuilding == null && _previewsBelts.Count > 1;

        for (var i = 0; i < _previewsBelts.Count - (continueDragging ? 1 : 0); ++i)
        {
            var belt = _previewsBelts[i].belt;
            gameMap.PutBuildingOnMap(belt);

            if (i != _previewsBelts.Count - 1)
                belt.outputBuilding = _previewsBelts[i + 1].belt;
            else
                belt.outputBuilding = _endpointBuilding;

            if (i == 0)
            {
                // TODO 更通用的方法
                _startpointBuilding?.OutputTo(_previewsBelts[0].belt);
            }

            Destroy(_previewsBelts[i].guideBlock.gameObject);
        }

        if (continueDragging)
        {
            var newBeginBelt = _previewsBelts[_previewsBelts.Count - 1];
            _previewsBelts.Clear();
            _previewsBelts.Add(newBeginBelt);
        }
        else
        {
            _previewsBelts.Clear();
            OnDraggingCancelled();
        }
    }

    /// <summary>
    /// 取消拖动，但是仍然在建造
    /// </summary>
    void OnDraggingCancelled()
    {
        var beginBelt
            = _previewsBelts.Count > 0 ? _previewsBelts[0] : GeneratePreviewBelt();

        for (var i = 1; i < _previewsBelts.Count; ++i)
        {
            Destroy(_previewsBelts[i].belt.gameObject);
            Destroy(_previewsBelts[i].guideBlock.gameObject);
        }

        _hasChosenStartPos = false;
        _endpointBuilding = null;
        _startpointBuilding = null;
        _previewsBelts.Clear();
        _previewsBelts.Add(beginBelt);
    }

    /// <summary>
    /// 取消建造
    /// </summary>
    void OnCancelled()
    {
        OnFinish();
    }
}
