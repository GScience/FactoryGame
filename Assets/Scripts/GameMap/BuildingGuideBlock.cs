using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BuildingGuideBlock : MonoBehaviour
{
    private SpriteRenderer _renderer;

    public Sprite canPlaceSprite;
    public Sprite canNotPlaceSprite;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        _renderer.sortingOrder = short.MaxValue;
    }
    public void SetCanPlace(bool canPlace)
    {
        _renderer.sprite = canPlace ? canPlaceSprite : canNotPlaceSprite;
    }
}
