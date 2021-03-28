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

    private void Start()
    {
        foreach (var fileName in Directory.EnumerateFiles(Application.persistentDataPath, "*.sav"))
        {
            var fileInfo = new FileInfo(fileName);
            var newElement = Instantiate(listElement, list);
            newElement.BindSaveData(fileInfo.Name.Substring(0, fileInfo.Name.Length - ".sav".Length));
        }
    }
}
