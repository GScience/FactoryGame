using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

/// <summary>
/// 弹出对话框
/// </summary>
[RequireComponent(typeof(PopMenu))]
public class SimpleMessageBox : MonoBehaviour
{
    [HideInInspector]
    [NonSerialized]
    public string message;

    public TextMeshProUGUI text;

    private PopMenu _popMenu;

    private void Awake()
    {
        _popMenu = GetComponent<PopMenu>();
        text.text = "";
    }
    private void Start()
    {
        if (string.IsNullOrEmpty(message))
            _popMenu.Close();

        text.text = message;
    }
}
