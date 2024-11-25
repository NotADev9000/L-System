using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

[Serializable]
public class MasterModel
{
    // Events subscribed to by UI controllers
    public static event Action<char> OnSymbolRemoved;
    public static event Action<int> OnIterationsUpdated;
    public static event Action<float> OnAngleChanged;
    public static event Action<float> OnAngleOffsetChanged;
    public static event Action<char, char> OnSymbolIdUpdated;
    public static event Action<char, TurtleFunction> OnFunctionUpdated;
    public static event Action<char, bool> OnIsVariableUpdated;

    [SerializeField] private int _iterations;
    public int Iterations { get { return _iterations; } set { _iterations = value; } }
    [SerializeField] private int _minIterationsAllowed = 1;
    [SerializeField] private int _maxIterationsAllowed = 10;

    [SerializeField] private float _angle;
    public float Angle { get { return _angle; } set { _angle = value; } }

    [SerializeField] private float _angleOffset;
    public float AngleOffset { get { return _angleOffset; } set { _angleOffset = value; } }

    [SerializeField] private string _axiom = string.Empty;
    public string Axiom { get { return _axiom; } set { _axiom = value; } }

    [SerializedDictionary("ID", "Data")]
    [SerializeField] private SerializedDictionary<char, DataSymbol> _symbols = new();
    public SerializedDictionary<char, DataSymbol> Symbols { get { return _symbols; } private set { _symbols = value; } }

    public MasterModel(int iterations, float angle, float angleOffset, SerializedDictionary<char, DataSymbol> symbols, string axiom)
    {
        Iterations = Mathf.Max(iterations, _minIterationsAllowed);
        Angle = angle;
        AngleOffset = angleOffset;
        Symbols = symbols;
        Axiom = axiom;
    }

    private void AddSymbol(char id, DataSymbol dataSymbol)
    {
        if (!Symbols.ContainsKey(id) && id != Char.MinValue)
        {
            Symbols.Add(id, dataSymbol);
        }
    }

    public void RemoveSymbol(char id, bool invokeEvent = true)
    {
        if (Symbols.ContainsKey(id))
        {
            if (Symbols.Remove(id) && invokeEvent)
                OnSymbolRemoved?.Invoke(id);
        }
    }

    public void UpdateIterations(int iterations, bool notify = true)
    {
        _iterations = Mathf.Clamp(iterations, _minIterationsAllowed, _maxIterationsAllowed);
        if (notify)
            OnIterationsUpdated?.Invoke(_iterations);
    }

    public void UpdateAngleByAmount(float change)
    {
        _angle += change;
        OnAngleChanged?.Invoke(_angle);
    }

    public void UpdateAngleOffsetByAmount(float change)
    {
        _angleOffset += change;
        OnAngleOffsetChanged?.Invoke(_angleOffset);
    }

    public void UpdateSymbolId(char oldId, char newId, DataSymbol dataSymbol)
    {
        RemoveSymbol(oldId, newId == Char.MinValue);
        AddSymbol(newId, dataSymbol);
        OnSymbolIdUpdated?.Invoke(oldId, newId);
    }

    public void UpdateSymbolFunction(char id, TurtleFunction newFunction)
    {
        if (Symbols.ContainsKey(id))
        {
            Symbols[id].TurtleFunction = newFunction;
            OnFunctionUpdated?.Invoke(id, newFunction);
        }
    }

    public void UpdateSymbolIsVariable(char id, bool isVariable)
    {
        if (Symbols.ContainsKey(id))
        {
            Symbols[id].IsVariable = isVariable;
            OnIsVariableUpdated?.Invoke(id, isVariable);
        }
    }
}
