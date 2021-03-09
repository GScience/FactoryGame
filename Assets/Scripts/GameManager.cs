using System;
using System.Collections.Generic;
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

    public bool IsPlaying { get; private set; }

    private void Awake()
    {
        if (InstanceHelper<GameManager>.GetGlobal() != this)
            Destroy(gameObject);
        GlobalGameManager = new InstanceHelper<GameManager>(this);
        DontDestroyOnLoad(this);

#if UNITY_EDITOR
        if (SceneManager.GetActiveScene().name == "GameScene")
            StartGame();
#endif
    }

    private void Update()
    {
        if (!IsPlaying)
            return;
        _timeSystem.Update();
        _moneySystem.Update();
    }

    public void StartGame()
    {
        if (IsPlaying)
            return;
        IsPlaying = true;
        _timeSystem = new TimeSystem();
        _moneySystem = new MoneySystem();
#if UNITY_EDITOR
        if (SceneManager.GetActiveScene().name != "GameScene")
#endif
            SceneManager.LoadScene("GameScene");
    }

    public void ExitGame()
    {
        IsPlaying = false;
        _timeSystem = null;
        _moneySystem = null;
    }
}
