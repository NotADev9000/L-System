using System;
using System.Collections;
using System.Collections.Generic;

public class MasterModel
{
    // Events subscribed to by UI controllers
    public static event Action<char> OnSymbolRemoved;
    public static event Action<char, char> OnSymbolIdUpdated;
    public static event Action<char, TurtleFunction> OnFunctionUpdated;
    public static event Action<char, bool> OnIsVariableUpdated;

    public string Axiom { get; private set; }
    public Dictionary<char, DataSymbol> Symbols { get; private set; } = new();

    public MasterModel(string axiom, Dictionary<char, DataSymbol> symbols)
    {
        Axiom = axiom;
        Symbols = symbols;
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
