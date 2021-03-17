using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 分配器端口设置按钮
/// </summary>
[RequireComponent(typeof(Image))]
public class DistributorPortStateButton : MonoBehaviour, IPointerClickHandler
{
    public Sprite inSprite;
    public Sprite outSprite;
    public Sprite disabledSprite;
    
    public Material inMaterial;
    public Material outMaterial;
    public Material disabledMaterial;

    public Distributor.PortType PortType { get; set; }

    public Action<Distributor.PortType> onPortTypeChanged;

    private Image _image;

    public void OnPointerClick(PointerEventData eventData)
    {
        ++PortType;
        if (PortType == Distributor.PortType.Unknown)
            PortType = Distributor.PortType.Disabled;

        onPortTypeChanged?.Invoke(PortType);
    }

    void Awake()
    {
        _image = GetComponent<Image>();
    }

    void Update()
    {
        switch (PortType)
        {
            case Distributor.PortType.Disabled:
                _image.sprite = disabledSprite;
                _image.material = disabledMaterial;
                break;
            case Distributor.PortType.In:
                _image.sprite = inSprite;
                _image.material = inMaterial;
                break;
            case Distributor.PortType.Out:
                _image.sprite = outSprite;
                _image.material = outMaterial;
                break;
        }
    }
}
