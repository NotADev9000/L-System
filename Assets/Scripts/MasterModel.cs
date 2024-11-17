using System.Collections;
using System.Collections.Generic;

public class MasterModel
{
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

    private void UpdateRule(char id, string successor)
    {
        if (Rules.ContainsKey(id))
        {
            Rules[id] = new DataRule(successor);
        }
    }
}
