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

    private Dictionary<string, ISystem> _systems = new Dictionary<string, ISystem>();

    public static TimeSystem TimeSystem => GlobalGameManager.Get()._systems["time"] as TimeSystem;
    public static MoneySystem MoneySystem => GlobalGameManager.Get()._systems["money"] as MoneySystem;
    public static CameraSystem CameraSystem => GlobalGameManager.Get()._systems["camera"] as CameraSystem;

    public static int SaveVersion = 1;
    public const string Magic = "FGS";

    /// <summary>
    /// 是否正在游戏
    /// </summary>
    public bool IsPlaying { get; private set; }
    
    /// <summary>
    /// 存档名称
    /// </summary>
    public string SaveName { get; private set; }

    private void Awake()
    {
        if (InstanceHelper<GameManager>.GetGlobal() != this)
            Destroy(gameObject);
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

        IsPlaying = true;

        _systems["time"] = new TimeSystem();
        _systems["money"] = new MoneySystem();
        _systems["camera"] = new CameraSystem();

#if UNITY_EDITOR
        if (SceneManager.GetActiveScene().name != "GameScene")
#endif

            yield return SceneManager.LoadSceneAsync("GameScene");

        yield return new WaitForEndOfFrame();

        if (!IsNewGame())
            LoadGame();
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
}
