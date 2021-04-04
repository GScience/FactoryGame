using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;

public class MoneySystem : ISystem
{
    public int Money { get; private set; } =
#if UNITY_EDITOR
        10000
#else
        5500
#endif
        ;

    /// <summary>
    /// 月中显示财务报表
    /// </summary>
    private float _lastShowMoneyDetailTime = -1;
    private int _lastShowMoneyDetailMoney;

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
        // 游戏统计出售物品
        GameManager.StatsSystem.OnSellItem(info, 1);
        Money += (int)(info.basePrice * 0.8f);
        return true;
    }

    public void Update()
    {
        if (GameManager.TimeSystem.Day == 3)
        {
            var totalTime = GameManager.TimeSystem.TotalTime;
            if (totalTime - _lastShowMoneyDetailTime < 30)
                return;
            var deltaTime = totalTime - _lastShowMoneyDetailTime;
            var deltaMoney = Money - _lastShowMoneyDetailMoney;
            if (deltaMoney > 0)
                GameManager.ShowToastMessage("财务小助手", (int)deltaTime + " 小时来盈利 " + deltaMoney);
            else
                GameManager.ShowToastMessage("财务小助手", (int)deltaTime + " 小时来亏损 " + -deltaMoney);
            _lastShowMoneyDetailTime = totalTime;
            _lastShowMoneyDetailMoney = Money;
        }
    }

    public void Init()
    {

    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(Money);
    }

    public void Load(BinaryReader reader)
    {
        Money = reader.ReadInt32();
        _lastShowMoneyDetailMoney = Money;
        _lastShowMoneyDetailTime = GameManager.TimeSystem.TotalTime;
    }
}
