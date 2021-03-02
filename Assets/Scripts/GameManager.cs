using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static InstanceHelper<GameManager> GlobalGameManager;

    public int Money { get; private set; } = 50000000;
    
    /// <summary>
    /// 总时间
    /// </summary>
    public float TotalTime { get; private set; } = 0;

    /// <summary>
    /// 周期
    /// </summary>
    public int Cycle { get => ((int)TotalTime / 720); }

    /// <summary>
    /// 天
    /// </summary>
    public int Day { get => ((int)TotalTime / 24) - Cycle * 30; }

    /// <summary>
    /// 小时
    /// </summary>
    public int Hour { get => ((int)TotalTime) - Cycle * 720 - Day * 24; }

    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI timeText;

    private void Awake()
    {
        GlobalGameManager = new InstanceHelper<GameManager>(this);
    }

    public bool RequireMoney(int money)
    {
        if (!HasEnoughMoney(money))
            return false;
        Money -= money;
        return true;
    }

    public bool HasEnoughMoney(int money)
    {
        return Money >= money;
    }

    public bool TrySellItem(ItemInfo info)
    {
        Money += (int)(info.basePrice * 0.8f);
        return true;
    }

    private void Update()
    {
        moneyText.text = Money.ToString();
        timeText.text = Cycle + "周期 " + Day + "天 " + Hour + "小时";

        TotalTime += Time.deltaTime * 1f;
    }
}
