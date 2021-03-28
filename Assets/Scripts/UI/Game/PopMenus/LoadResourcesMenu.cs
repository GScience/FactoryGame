using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

/// <summary>
/// 加载资源
/// </summary>
[RequireComponent(typeof(PopMenu))]
public class LoadResourcesMenu : MonoBehaviour
{
    public TextMeshProUGUI progressText;

    public void SetProgress(string progress)
    {
        progressText.text = progress;
    }
}
