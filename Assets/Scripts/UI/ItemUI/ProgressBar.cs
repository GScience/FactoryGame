using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Image progressBar;
    public Image progressBarMask;
    public Image progressBarBorder;

    public float Percentage
    {
        set
        {
            progressBar.fillAmount = value;
        }
    }

    private void OnEnable()
    {
        progressBarBorder.enabled = true;
        progressBarMask.enabled = true;
        progressBar.enabled = true;
    }

    private void OnDisable()
    {
        progressBarBorder.enabled = false;
        progressBarMask.enabled = false;
        progressBar.enabled = false;
    }
}
