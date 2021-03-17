using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BuildingInputGuide : MonoBehaviour
{
    public GameObject inputGuideGroup;
    private bool _visiable = false;

    private void Update()
    {
        if (!_visiable)
            return;

        if (inputGuideGroup != null)
            inputGuideGroup.SetActive(GameMap.GlobalMap.Get().isBuilding);
    }

    public void OnBecameVisible()
    {
        _visiable = true;
    }

    public void OnBecameInvisible()
    {
        _visiable = false;
    }
}
