using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 资源管理器
/// 负责加载资源
/// </summary>
public class ResourcesManager : MonoBehaviour
{
    /// <summary>
    /// 所有ab包的名称
    /// </summary>
    private static readonly string[] _abNames = new []{ "gamepack.core" };

    private static Dictionary<int, BuildingBase> _buildingDatabase;
    private static List<BuildingCardBase> _cardDatabase;
    private static Dictionary<int, ScriptableObject> _scriptableDatabase;

    private LoadResourcesMenu _loadResourceMenu;
    private IEnumerator _current;

    private bool _hasInit;

    public void Init()
    {
        if (_current == null)
            _current = InitAsync();
        while (_current.MoveNext()) ;
    }

    private IEnumerator InitAsync()
    {
        if (_buildingDatabase != null)
            yield break;

        var loadResourcesPopMenu = PopMenuLayer.GlobalPopMenuLayer.Get().Pop("LoadResourcesMenu");
        _loadResourceMenu = loadResourcesPopMenu.GetComponent<LoadResourcesMenu>();

        _buildingDatabase = new Dictionary<int, BuildingBase>();
        _cardDatabase = new List<BuildingCardBase>();
        _scriptableDatabase = new Dictionary<int, ScriptableObject>();

        foreach (var abName in _abNames)
        {
            var loadAB = LoadAssetBundleAsync(abName);
            while (loadAB.MoveNext())
                yield return 0;
        }

        loadResourcesPopMenu.Close();
        _hasInit = true;
    }

    private void AddToDatabase(UnityEngine.Object asset, string name)
    {
        if (asset is GameObject gameObject)
        {
            var buildingBase = gameObject.GetComponent<BuildingBase>();
            if (buildingBase != null)
            {
                _buildingDatabase[name.GetHashCode()] = buildingBase;
                buildingBase.NameHash = name.GetHashCode();
                Debug.Log("Building " + name + " loaded");
                return;
            }

            var buildingCardBase = gameObject.GetComponent<BuildingCardBase>();
            if (buildingCardBase != null)
            {
                _cardDatabase.Add(buildingCardBase);
                Debug.Log("Card " + name + " loaded");
                return;
            }
        }
        else if (asset is NamedScriptableObject scriptableObject)
        {
            _scriptableDatabase[name.GetHashCode()] = scriptableObject;
            scriptableObject.NameHash = name.GetHashCode();
            Debug.Log("Scriptable object " + name + " loaded");
        }
    }

    private IEnumerator LoadAssetBundleAsync(string abName)
    {
#if UNITY_EDITOR
        _loadResourceMenu.SetProgress("Loading from editor...");
        var paths = AssetDatabase.GetAssetPathsFromAssetBundle(abName);
        foreach (var path in paths)
        {
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            AddToDatabase(asset, path.ToLower());
        }

#else
        _loadResourceMenu.SetProgress("Loading " + abName + "...");

        var pathName = Application.dataPath + "/AssetBundles/" + abName;
        if (!File.Exists(pathName))
        {
            Debug.LogWarning("Asset bundles " + pathName + " not found");
            yield break;
        }
        
        Debug.Log("Loading asset bundles " + abName);
        var abLoadResult = AssetBundle.LoadFromFileAsync(pathName);
        yield return abLoadResult;
        var ab = abLoadResult.assetBundle;
        if (ab == null)
        {
            Debug.LogWarning("Failed to load " + abName);
            yield break;
        }
        var names = ab.GetAllAssetNames();

        foreach (var name in names)
        {
            _loadResourceMenu.SetProgress("Loading " + abName + ": " + name + "...");
            var assetLoadResult = ab.LoadAssetAsync<UnityEngine.Object>(name);

            yield return assetLoadResult;
            AddToDatabase(assetLoadResult.asset, name.ToLower());
        }
        Debug.Log(names.Length + " assets are loaded");
#endif
        yield break;
    }

    private static void CheckInit()
    {
        if (!InstanceHelper<ResourcesManager>.GetGlobal()._hasInit)
            InstanceHelper<ResourcesManager>.GetGlobal().Init();
    }

    public static List<BuildingCardBase> GetAllCards()
    {
        CheckInit();
        return _cardDatabase;
    }

    public static BuildingBase GetBuilding(int keyHash)
    {
        CheckInit();
        if (_buildingDatabase.TryGetValue(keyHash, out var result))
            return result;
        return null;
    }

    public void Start()
    {
        _current = InitAsync();
        StartCoroutine(_current);
    }
}
