using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 物品需求UI
/// 在任务、合成配方里都会用到
/// </summary>
public class ItemRequirementUI : MonoBehaviour
{
    public ItemStackUI itemUI;
    public TextMeshProUGUI text;
    public ProgressBar progressBar;

    /// <summary>
    /// 需求数量
    /// </summary
    [NonSerialized]
    [HideInInspector]
    public int requireCount;
    
    /// <summary>
    /// 拥有数量
    /// </summary>
    [NonSerialized]
    [HideInInspector]
    public int haveCount;

    /// <summary>
    /// 需要的物品
    /// </summary>
    [NonSerialized]
    [HideInInspector]
    public ItemInfo needItem;

    void Update()
    {
        if (needItem == null)
        {
            itemUI.gameObject.SetActive(false);
            text.enabled = false;
            progressBar.enabled = false;
            return;
        }

        itemUI.gameObject.SetActive(true);
        text.enabled = true;
        progressBar.enabled = true;

        text.text = haveCount + "/" + requireCount;
        itemUI.Item = needItem;
        progressBar.Percentage = haveCount / (float)requireCount;
    }
}
