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

    private TimeSystem _timeSystem;
    private MoneySystem _moneySystem;

    public static TimeSystem TimeSystem => GlobalGameManager.Get()._timeSystem;
    public static MoneySystem MoneySystem => GlobalGameManager.Get()._moneySystem;

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
        _timeSystem.Update();
        _moneySystem.Update();
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
        _timeSystem = new TimeSystem();
        _moneySystem = new MoneySystem();

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
            writer.Write(Magic);
            writer.Write(SaveVersion);

            InstanceHelper<GameMap>.GetGlobal().Save(writer);
            TimeSystem.Save(writer);
            MoneySystem.Save(writer);
        }
    }

    private void LoadGame()
    {
        var file = File.OpenRead(Application.persistentDataPath + "/" + SaveName + ".sav");
        var decompressStream = new GZipStream(file, CompressionMode.Decompress);

        using (var reader = new BinaryReader(decompressStream))
        {
            if (Magic != reader.ReadString() ||
                SaveVersion != reader.ReadInt32())
                return;

            InstanceHelper<GameMap>.GetGlobal().Load(reader);
            TimeSystem.Load(reader);
            MoneySystem.Load(reader);
        }
    }
}
