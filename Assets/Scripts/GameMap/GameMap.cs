using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Grid))]
public class GameMap : MonoBehaviour
{
    private Grid _grid;

    /// <summary>
    /// 把刷新器和建筑分类到一起
    /// </summary>
    private class UpdaterGroup
    {
        private readonly IBuildingUpdater _updater;
        private readonly List<BuildingBase> _buildings;

        public UpdaterGroup(IBuildingUpdater updater)
        {
            _updater = updater;
            _buildings = new List<BuildingBase>();
        }

        public void Add(BuildingBase building)
        {
            _buildings.Add(building);
        }

        public void Update(Action<BuildingBase, IBuildingUpdater> func)
        {
            foreach (var building in _buildings)
                func(building, _updater);
        }

        public void Remove(BuildingBase building)
        {
            _buildings.Remove(building);
        }
    }

    /// <summary>
    /// 所有建筑
    /// </summary>
    private Dictionary<int, BuildingBase> _buildings = new Dictionary<int, BuildingBase>();

    /// <summary>
    /// 用来查找刷新组
    /// </summary>
    private Dictionary<string, UpdaterGroup> _updaterGroups = new Dictionary<string, UpdaterGroup>();

    /// <summary>
    /// 用来查找建筑
    /// </summary>
    private Dictionary<Vector2Int, BuildingBase> _buildingMap = new Dictionary<Vector2Int, BuildingBase>();

    /// <summary>
    /// 禁止点击地图上的建筑
    /// </summary>
    [HideInInspector]
    [NonSerialized]
    public bool disableClickWithMapElement = false;

    public static InstanceHelper<GameMap> GlobalMap;

    void Awake()
    {
        GlobalMap = new InstanceHelper<GameMap>(this);
        _grid = GetComponent<Grid>();
    }

    void Start()
    {
        // 获取所有刷新器并且创建刷新组
        foreach (var type in GetType().Assembly.GetTypes())
        {
            if (!typeof(IBuildingUpdater).IsAssignableFrom(type) || type.IsAbstract)
                continue;

            if (!type.Name.EndsWith("Updater"))
                continue;

            var updater = Activator.CreateInstance(type) as IBuildingUpdater;
            _updaterGroups.Add(type.Name.Substring(0, type.Name.Length - "Updater".Length), new UpdaterGroup(updater));
        }
    }

    public void PutBuildingOnMap(BuildingBase building)
    {
#if UNITY_EDITOR
        if (!building.IsPreviewMode)
            Debug.LogError("Only can put preview building on map");
#endif
        AddBuilding(building);
    }

    public void DestroyBuilding(BuildingBase building)
    {
        var pos = building.GetComponent<GridElement>().CellPos;

        // 修改连接到这个建筑的输出建筑的输出为null
        if (building is IBuildingCanInputItem inputBuilding)
        {
            var inputsFrom = inputBuilding.GetInputBuildings();
            foreach (var inputFrom in inputsFrom)
                inputFrom?.TrySetOutputTo(null, pos);
        }

        // 修改连接到这个建筑的输入建筑的输出为null
        if (building is IBuildingCanOutputItem outputBuilding)
        {
            var outputsTo = outputBuilding.GetOutputBuildings();
            foreach (var outputTo in outputsTo)
                outputTo?.TrySetInputFrom(null, pos);
        }

        // 移除
        _buildings.Remove(building.id);
        var gridElement = building.GetComponent<GridElement>();
        var cellPos = gridElement.CellPos;
        var size = gridElement.Size;
        for (var x = 0; x < size.x; ++x)
            for (var y = 0; y < size.y; ++y)
                _buildingMap.Remove(cellPos + new Vector2Int(x, y));
        var updater = building.UpdaterRef;
        var updateGroup = _updaterGroups[updater.className];
        updateGroup.Remove(building);
        Destroy(building.gameObject);
    }

    public BuildingBase CreateBuildingPreview(BuildingBase building)
    {
        var newBuilding = Instantiate(building, transform);
        newBuilding.EnterPreviewMode();
        return newBuilding;
    }

    public BuildingBase GetBuildingAt(Vector2Int pos)
    {
        if (_buildingMap.TryGetValue(pos, out var building))
            return building;
        return null;
    }
    private void AddBuilding(BuildingBase building)
    {
        building.ExitPreviewMode();
        building.id = _buildings.Count;
        _buildings[_buildings.Count] = building;
        var updaterName = building.UpdaterRef.className;

        if (_updaterGroups.TryGetValue(updaterName, out var updaterGroup))
            updaterGroup.Add(building);
        else
            Debug.LogError("Failed to find updater " + updaterName);

        var gridElement = building.GetComponent<GridElement>();
        var cellPos = gridElement.CellPos;
        var size = gridElement.Size;
        for (var x = 0; x < size.x; ++x)
            for (var y = 0; y < size.y; ++y)
                _buildingMap[cellPos + new Vector2Int(x, y)] = building;
    }

    /// <summary>
    /// 上一个鼠标所在的建筑
    /// </summary>
    private BuildingBase _lastMouseOverBuilding;

    void Update()
    {
        // 循环刷新建筑
        foreach (var pair in _updaterGroups)
        {
            var updaterGroup = pair.Value;
            updaterGroup.Update(BuildingUpdate);
        }

        // 判断点击
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            var viewportPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var cellPos = (Vector2Int)_grid.WorldToCell(viewportPos);
            if (_buildingMap.TryGetValue(cellPos, out var building))
            {
                if (building != _lastMouseOverBuilding)
                {
                    if (_lastMouseOverBuilding != null)
                        _lastMouseOverBuilding.OnMouseLeave();
                    _lastMouseOverBuilding = building;
                    building.OnMouseEnter();
                }

                // 点击
                if (!disableClickWithMapElement && PlayerInput.GetMouseClick(0))
                    building.OnClick();
            }
            else
            {
                _lastMouseOverBuilding?.OnMouseLeave();
                _lastMouseOverBuilding = null;
            }
        }
        else
        {
            _lastMouseOverBuilding?.OnMouseLeave();
            _lastMouseOverBuilding = null;
        }
    }

    /// <summary>
    /// 刷新建筑，包括合成配方状态等等
    /// </summary>
    /// <param name="building"></param>
    /// <param name="updater"></param>
    void BuildingUpdate(BuildingBase building, IBuildingUpdater updater)
    {
        updater.OnUpdate(building);
    }

#if UNITY_EDITOR
    private IEnumerable<string> _updaters;

    public IEnumerable<string> RefreshBuildingUpdaters()
    {
        return _updaters = from type in GetType().Assembly.GetTypes()
            where typeof(IBuildingUpdater).IsAssignableFrom(type) && !type.IsAbstract
            select type.Name;
    }

    public IEnumerable<string> GetBuildingUpdaters()
    {
        if (_updaters == null)
            RefreshBuildingUpdaters();
        return _updaters;
    }
#endif
}
