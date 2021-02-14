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
    }

    /// <summary>
    /// 所有建筑
    /// </summary>
    private List<BuildingBase> _buildings = new List<BuildingBase>();

    /// <summary>
    /// 用来查找刷新组
    /// </summary>
    private Dictionary<string, UpdaterGroup> _updaterGroups = new Dictionary<string, UpdaterGroup>();

    /// <summary>
    /// 用来查找建筑
    /// </summary>
    private Dictionary<Vector2Int, BuildingBase> _buildingMap = new Dictionary<Vector2Int, BuildingBase>();

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

        StartCoroutine(GenerateTestBuilding());
    }

    public BuildingBase testBuilding;

    private IEnumerator GenerateTestBuilding()
    {
        // 测试！添加10个建筑测试效率
        for (var i = 0; i < 10; ++i)
        {
            var previewBuilding = CreateBuildingPreview(testBuilding);
            previewBuilding.transform.position = new Vector3(i * 3, 0, 0);
            var factory = previewBuilding as Factory;
            if (factory == null)
                continue;
            factory.SetCurrentRecipe(0);
            if (i == 0)
            {
                for (var j = 0; j < factory.CurrentRecipe.input.Count; ++j)
                    for (var k = 0; k < factory.CurrentRecipe.input[i].count; ++k)
                        factory.TryPutOneItem(factory.CurrentRecipe.input[i].item);
            }
            
            if (i > 0)
                (_buildings[i - 1] as Factory).outputBuilding = factory;

            PutBuildingOnMap(previewBuilding);

            Debug.Log(i);

            if (i % 50 == 0)
                yield return 0;
        }
        (_buildings[_buildings.Count - 1] as Factory).outputBuilding = _buildings[0] as Factory;
    }

    public void PutBuildingOnMap(BuildingBase building)
    {
#if UNITY_EDITOR
        if (!building.IsPreviewMode)
            Debug.LogError("Only can put preview building on map");
#endif
        AddBuilding(building);
    }

    public BuildingBase CreateBuildingPreview(BuildingBase building)
    {
        var newBuilding = Instantiate(building, transform);
        newBuilding.EnterPreviewMode();
        return newBuilding;
    }

    private void AddBuilding(BuildingBase building)
    {
        building.ExitPreviewMode();
        building.id = _buildings.Count;
        _buildings.Add(building);
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
            }
            else if (_lastMouseOverBuilding != null)
            {
                _lastMouseOverBuilding.OnMouseLeave();
                _lastMouseOverBuilding = null;
            }
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
