using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DestoryButton : MonoBehaviour
{
    private BuildingElementSelector _selector;

    public void Start()
    {
        _selector = BuildingElementSelector.GlobalSelector.Get();
    }

    public void OnClick()
    {
        _selector.Close();
        BuildingDestroyer.GlobalBuilder.Get().Open(_selector.Open);
    }
}
