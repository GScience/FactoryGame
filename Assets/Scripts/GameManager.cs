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

    public int Money { get; private set; } =
#if UNITY_EDITOR
        100000000
#else
        5500
#endif
        ;

    /// <summary>
    /// 总时间
    /// </summary>
    public float TotalTime { get; private set; } = 0;

    /// <summary>
    /// 周期
    /// </summary>
    public int Week { get => ((int)TotalTime / 168); }

    /// <summary>
    /// 天
    /// </summary>
    public int Day { get => ((int)TotalTime / 24) - Week * 7; }

    /// <summary>
    /// 小时
    /// </summary>
    public int Hour { get => ((int)TotalTime) - Week * 168 - Day * 24; }

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
        timeText.text = Week + "周 " + Day + "天 " + Hour + "小时";

        TotalTime += Time.deltaTime * 1f;
    }
}
