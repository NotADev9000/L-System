using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DataLine
{
    [SerializeField] private float _length;
    public float Length { get { return _length; } }

    [SerializeField] private bool _isVisible;
    public bool IsVisible { get { return _isVisible; } }

    [SerializeField] private Material _color;
    public Material Color { get { return _color; } }

    [SerializeField] private GameObject _leafPrefab;
    public GameObject LeafPrefab { get { return _leafPrefab; } }

    public DataLine(float length = 1, bool isVisible = true, Material color = null, GameObject leafPrefab = null)
    {
        _length = length;
        _isVisible = isVisible;
        _color = color ?? MaterialsManager.Instance.Materials[0];
        _leafPrefab = leafPrefab;
    }

    public void UpdateLength(float newLength)
    {
        _length = newLength;
    }

    public void UpdateVisibility(bool isVisible)
    {
        _isVisible = isVisible;
    }

    public void UpdateColor(Material newColor)
    {
        _color = newColor;
    }
}