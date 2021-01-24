using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingElementCard : MonoBehaviour, IPointerClickHandler
{
    public string factoryName;

    public void OnPointerClick(PointerEventData eventData)
    {
        var selector = BuildingElementSelector.GlobalSelector.Get();

        selector.Close();

        var testObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var factory = testObj.AddComponent<Factory>();
        factory.factoryName = factoryName;
        BuildingPicker.GlobalPicker.Get().Pick(factory, 
            () =>
            {
                selector.Open();
            }, 
            () =>
            {
                Destroy(testObj);
                selector.Open();
            });
    }
}
