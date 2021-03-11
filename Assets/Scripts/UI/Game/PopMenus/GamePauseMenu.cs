using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(PopMenu))]
public class GamePauseMenu : MonoBehaviour
{
    private void OnEnable()
    {
        Time.timeScale = 0;
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
    }

    public void SaveGame()
    {
        InstanceHelper<GameManager>.GetGlobal().SaveGame();
        var msgBox = PopMenuLayer.GlobalPopMenuLayer.Get().Pop("SimpleMessageBox").GetComponent<SimpleMessageBox>();
        msgBox.message = "保存成功！";
    }

    public void QuitGame()
    {
        InstanceHelper<GameManager>.GetGlobal().QuitGame();
    }
}
