using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(PopMenu))]
public class LoadGameMenu : MonoBehaviour
{
    public LoadGameMenuElement listElement;
    public Transform list;

    private List<LoadGameMenuElement> _saveElements = new List<LoadGameMenuElement>();

    private void Start()
    {
        foreach (var fileName in Directory.EnumerateFiles(Application.persistentDataPath, "*.sav"))
        {
            var fileInfo = new FileInfo(fileName);
            var newElement = Instantiate(listElement, list);
            newElement.BindSaveData(fileInfo.Name.Substring(0, fileInfo.Name.Length - ".sav".Length));
            _saveElements.Add(newElement);
        }

        // 排序
        _saveElements.Sort((element1, element2) =>
        {
            var deltaTime = element1.LastModityTime - element2.LastModityTime;
            if (deltaTime.Ticks > 0)
                return -1;
            else if (deltaTime.Ticks < 0)
                return 1;
            return 0;
        });

        for (var i = 0; i < _saveElements.Count; ++i)
            _saveElements[i].transform.SetSiblingIndex(i);
    }
}
