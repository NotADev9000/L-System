using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLine
{
    private float _length;
    public float Length { get { return _length; } }

    private bool _isVisible;
    public bool IsVisible { get { return _isVisible; } }

    private Color _color;
    public Color Color { get { return _color; } }

    private GameObject _leafPrefab;
    public GameObject LeafPrefab { get { return _leafPrefab; } }

    public DataLine(float length = 1, bool isVisible = true, Color color = default, GameObject leafPrefab = null)
    {
        _length = length;
        _isVisible = isVisible;
        _color = color;
        _leafPrefab = leafPrefab;
    }
}