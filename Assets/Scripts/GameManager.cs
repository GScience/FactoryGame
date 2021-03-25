using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static InstanceHelper<GameManager> GlobalGameManager;

    private Dictionary<string, ISystem> _systems;

    public static TimeSystem TimeSystem => GlobalGameManager.Get()._systems["time"] as TimeSystem;
    public static MoneySystem MoneySystem => GlobalGameManager.Get()._systems["money"] as MoneySystem;
    public static CameraSystem CameraSystem => GlobalGameManager.Get()._systems["camera"] as CameraSystem;
    public static StageSystem StageSystem => GlobalGameManager.Get()._systems["stage"] as StageSystem;
    public static StatsSystem StatsSystem => GlobalGameManager.Get()._systems["stats"] as StatsSystem;

    public static int SaveVersion = 1;
    public const string Magic = "FGS";

    /// <summary>
    /// 任务UI
    /// </summary>
    public ObjectiveToast missionUI;

    /// <summary>
    /// 是否正在游戏
    /// </summary>
    public bool IsPlaying { get; private set; }
    
    /// <summary>
    /// 存档名称
    /// </summary>
    public string SaveName { get; private set; }

    /// <summary>
    /// 通用UI层
    /// </summary>
    private static Transform _uiLayer = null;

    private void Awake()
    {
        if (InstanceHelper<GameManager>.GetGlobal() != this)
        {
            Destroy(gameObject);
            return;
        }
        GlobalGameManager = new InstanceHelper<GameManager>(this);
        DontDestroyOnLoad(this);
#if UNITY_EDITOR
        if (SceneManager.GetActiveScene().name == "GameScene")
            StartGame("save0");
#endif
    }

    private void Update()
    {
        if (!IsPlaying)
            return;
        foreach (var system in _systems)
            system.Value.Update();
    }

    private static string GetSavePath(string saveName)
    {
        return Application.persistentDataPath + "/" + saveName + ".sav";
    }

    private bool IsNewGame()
    {
        return !File.Exists(GetSavePath(SaveName));
    }

    public void StartGame(string saveName)
    {
        StartCoroutine(StartGameAsync(saveName));
    }

    private IEnumerator StartGameAsync(string saveName)
    {
        if (IsPlaying)
            yield break;

        SaveName = saveName;

        _systems = new Dictionary<string, ISystem>()
        {
            { "time", new TimeSystem() },
            { "money",new MoneySystem() },
            { "camera",new CameraSystem() },
            { "stage", new StageSystem() },
            { "stats",new  StatsSystem() }
        };

        IsPlaying = true;

#if UNITY_EDITOR
        if (SceneManager.GetActiveScene().name != "GameScene")
#endif

            yield return SceneManager.LoadSceneAsync("GameScene");

        yield return new WaitForEndOfFrame();

        foreach (var system in _systems)
            system.Value.Init();

        if (!IsNewGame())
        {
            try
            {
                LoadGame();
                ShowToastMessage("工厂小助手", "欢迎您回来~");
            }
            catch (Exception e)
            {
                ShowToastMessage("工厂小助手", "在您不在的时候工厂受到了未知力量的破坏");
                Console.WriteLine(e.ToString());
            }
        }
    }

    public void QuitGame()
    {
        IsPlaying = false;
        SceneManager.LoadScene("MainMenuScene");
    }

    public void SaveGame()
    {
        var file = File.Create(Application.persistentDataPath + "/" + SaveName + ".sav");
        var compressStream = new GZipStream(file, CompressionMode.Compress);
        using (var writer = new BinaryWriter(compressStream))
        {
            writer.Write(Encoding.UTF8.GetBytes(Magic));
            writer.Write(SaveVersion);

            InstanceHelper<GameMap>.GetGlobal().Save(writer);
            foreach (var system in _systems)
            {
                writer.Write(system.Key);
                system.Value.Save(writer);
            }
            writer.Write("");
        }
    }

    private void LoadGame()
    {
        var file = File.OpenRead(Application.persistentDataPath + "/" + SaveName + ".sav");
        var decompressStream = new GZipStream(file, CompressionMode.Decompress);

        using (var reader = new BinaryReader(decompressStream))
        {
            var magic = Encoding.UTF8.GetString(reader.ReadBytes(Magic.Length));
            var saveVerison = reader.ReadInt32();
            if (Magic != magic ||
                SaveVersion != saveVerison)
                return;

            InstanceHelper<GameMap>.GetGlobal().Load(reader);

            var systemName = reader.ReadString();
            while (!string.IsNullOrEmpty(systemName))
            {
                if (_systems.TryGetValue(systemName, out var system))
                    system.Load(reader);
                systemName = reader.ReadString();
            }
        }
    }

    public static ObjectiveToast ShowToastMessage(string title, string message, AudioClip audio = null, float delay = 30f, bool canFadeOut = true)
    {
        var missionUI = GameManager.GlobalGameManager.Get().missionUI;
        if (_uiLayer == null)
            _uiLayer = GameObject.Find("UILayer").transform;
        var toast = GameObject.Instantiate(missionUI, _uiLayer);
        toast.Initialize(title, message, "", audio, delay, canFadeOut);

        return toast;
    }
}
