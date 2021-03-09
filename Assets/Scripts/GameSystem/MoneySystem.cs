using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;

public class MoneySystem : ISystem
{
    public int Money { get; private set; } =
#if UNITY_EDITOR
        100000000
#else
        5500
#endif
        ;

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

    public void Update()
    {
    }
}
