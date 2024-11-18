using System;
using System.Collections;
using System.Collections.Generic;

public class MasterModel
{
    // Events subscribed to by UI controllers
    public static event Action<char> OnSymbolRemoved;
    public static event Action<char, char> OnSymbolIdUpdated;

    public string Axiom { get; private set; }
    public Dictionary<char, DataSymbol> Symbols { get; private set; } = new();
    public Dictionary<char, DataRule> Rules { get; private set; } = new();

    public MasterModel(string axiom, Dictionary<char, DataSymbol> symbols, Dictionary<char, DataRule> rules)
    {
        Axiom = axiom;
        Symbols = symbols;
        Rules = rules;

        foreach (KeyValuePair<char, DataSymbol> kvp in symbols)
        {
            DataSymbol symbol = kvp.Value;

            // If passed in rules dictionary is missing a rule for a variable symbol, add an empty rule
            if (symbol.IsVariable && !rules.ContainsKey(kvp.Key))
            {
                Rules.Add(kvp.Key, new DataRule(""));
            }
        }
    }

    private void AddSymbol(char id, DataSymbol dataSymbol)
    {
        if (!Symbols.ContainsKey(id))
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
        RemoveSymbol(oldId, false);
        AddSymbol(newId, dataSymbol);
        OnSymbolIdUpdated?.Invoke(oldId, newId);
    }

    private void UpdateRule(char id, string successor)
    {
        if (Rules.ContainsKey(id))
        {
            Rules[id] = new DataRule(successor);
        }
    }
}
