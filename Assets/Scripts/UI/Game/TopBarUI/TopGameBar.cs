using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

/// <summary>
/// 游戏最上边的菜单栏
/// </summary>
public class TopGameBar : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI stageText;

    void Update()
    {
        timeText.text = GameManager.TimeSystem.ToString();
        moneyText.text = GameManager.MoneySystem.Money.ToString();
        stageText.text = GameManager.StageSystem.GetCurrentGameStageName();
    }

    private void Start()
    {
        PopMenuLayer.GlobalPopMenuLayer.Get().TryCloseButEmpty += () =>
        {
            if (!GameMap.GlobalMap.Get().isBuilding)
                ShowPauseMenu();
        };
    }

    public void ShowPauseMenu()
    {
        PopMenuLayer.GlobalPopMenuLayer.Get().Pop("GamePauseMenu");
    }
}
