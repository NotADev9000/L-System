using System;
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

    public DataLine(float length = 1, bool isVisible = true, Material color = null)
    {
        _length = length;
        _isVisible = isVisible;
        _color = color ?? MaterialsManager.Instance.Materials[0];
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