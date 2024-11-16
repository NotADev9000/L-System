using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLine
{
    private float _length;
    public float Length { get { return _length; } }

    private bool _isVisible;
    private Color _color;
    private GameObject _leafPrefab;

    public DataLine(float length, bool isVisible, Color color, GameObject leafPrefab)
    {
        _length = length;
        _isVisible = isVisible;
        _color = color;
        _leafPrefab = leafPrefab;
    }
}