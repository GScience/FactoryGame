using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ���ش浵�б�Ԫ��
/// </summary>
public class LoadGameMenuElement : MonoBehaviour, IPointerClickHandler
{
    private GameInfo _gameInfo;
    private string _saveName;

    /// <summary>
    /// �浵�����ı���
    /// </summary>
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dateText;

    public void BindSaveData(string saveName)
    {
        var jsonFilePath = Application.persistentDataPath + "/" + saveName + ".json";

        if (File.Exists(jsonFilePath))
            _gameInfo = JsonUtility.FromJson<GameInfo>(File.ReadAllText(jsonFilePath, Encoding.UTF8));
        else
        {
            _gameInfo = new GameInfo();
            _gameInfo.name = saveName;
        }

        _saveName = saveName;

        nameText.text = _gameInfo.name;
        dateText.text = _gameInfo.lastModifyTime;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.GlobalGameManager.Get().StartGame(_saveName);
    }
}
