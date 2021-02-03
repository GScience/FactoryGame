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
    public Material meshMaterial;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    void Start()
    {
        _renderer.material = meshMaterial;
    }

    void Update()
    {
        transform.position = center.position + offset;
    }

    void OnEnable()
    {
        _renderer.enabled = true;
    }

    void OnDisable()
    {
        _renderer.enabled = false;
    }
}
