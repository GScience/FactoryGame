using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class GameMap : MonoBehaviour
{
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
    private Dictionary<string, UpdaterGroup> _updaterGroups = new Dictionary<string, UpdaterGroup>();

    public static InstanceHelper<GameMap> GlobalMap;

    void Awake()
    {
        GlobalMap = new InstanceHelper<GameMap>(this);
    }

    void Start()
    {
        // 获取所有刷新器并且创建刷新组
        foreach (var type in GetType().Assembly.GetTypes())
        {
            if (!typeof(IBuildingUpdater).IsAssignableFrom(type) || type.IsAbstract)
                continue;

            var updater = Activator.CreateInstance(type) as IBuildingUpdater;
            _updaterGroups.Add(type.Name, new UpdaterGroup(updater));
        }

        StartCoroutine(GenerateTestBuilding());
    }

    public BuildingBase testBuilding;

    private IEnumerator GenerateTestBuilding()
    {
        // 测试！添加100000个建筑测试效率
        for (var i = 0; i < 10000; ++i)
        {
            var previewBuilding = CreateBuildingPreview(testBuilding);
            previewBuilding.transform.position = new Vector3(i * 3, 0, 0);
            var factory = previewBuilding as Factory;
            factory.currentRecipe = factory.recipes[0];
            if (i == 0)
                factory.currentRecipe.requirement.ForEach((o) =>
                    factory.inputItemCache.Add(new ItemStack {count = o.count, item = o.item}));
            else
                factory.InputBuilding = _buildings[i - 1] as IAcceptItemBuilding;

            PutBuildingOnMap(previewBuilding);

            Debug.Log(i);

            if (i % 50 == 0)
                yield return 0;
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
    }

    void Update()
    {
        foreach (var pair in _updaterGroups)
        {
            var updaterGroup = pair.Value;
            updaterGroup.Update(BuildingUpdate);
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
