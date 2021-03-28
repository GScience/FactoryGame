using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MainMenuButtonList : MonoBehaviour
{
    public void LoadGame()
    {
        PopMenuLayer.GlobalPopMenuLayer.Get().Pop("LoadGameMenu");
    }

    public void NewGame()
    {
        PopMenuLayer.GlobalPopMenuLayer.Get().Pop("NewGameMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
