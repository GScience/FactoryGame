using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class StartGameButton : MonoBehaviour
{
    public TMP_InputField inputField;

    public void StartGame()
    {
        InstanceHelper<GameManager>.GetGlobal().StartGame(inputField.text);
    }
}
