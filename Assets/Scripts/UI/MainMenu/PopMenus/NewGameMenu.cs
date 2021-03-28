using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PopMenu))]
public class NewGameMenu : MonoBehaviour
{
    /// <summary>
    /// 输入框
    /// </summary>
    public TMP_InputField inputField;

    public void OnConfirm()
    {
        var saveName = inputField.text;
        var gameManager = GameManager.GlobalGameManager.Get();
        if (!gameManager.IsNewGame(saveName))
        {
            var msgBox = PopMenuLayer.GlobalPopMenuLayer.Get().Pop("SimpleMessageBox").GetComponent<SimpleMessageBox>();
            msgBox.message = "存档已存在！";
        }
        else
            GameManager.GlobalGameManager.Get().StartGame(inputField.text);
    }
}
