using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class GridRenderer : MonoBehaviour
{
    private Renderer _renderer;
    public Transform center;
    public Vector3 offset = new Vector3(0, 10, 0);
    public Material meshMaterialSelected;
    public Material meshMaterialUnselected;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    void Start()
    {
        OnUnselected();
    }

    void Update()
    {
        transform.position = center.position + offset;
    }

    public void OnSelected()
    {
        _renderer.material = meshMaterialSelected;
    }

    public void OnUnselected()
    {
        _renderer.material = meshMaterialUnselected;
    }
}
