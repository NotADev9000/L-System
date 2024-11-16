using System.Collections;
using System.Collections.Generic;

public class MasterModel
{
    public string Axiom { get; private set; }
    public Dictionary<char, DataSymbol> Symbols { get; private set; } = new();
    public Dictionary<char, DataRule> Rules { get; private set; } = new();

    public MasterModel(string axiom, List<DataSymbol> symbols, Dictionary<char, DataRule> rulesDict)
    {
        Axiom = axiom;

        foreach (DataSymbol symbol in symbols)
        {
            // Convert list to dictionary using ID as key
            Symbols.Add(symbol.Id, symbol);

            if (symbol.IsVariable)
            {
                DataRule rule = rulesDict[symbol.Id] ?? new DataRule("");
                Rules.Add(symbol.Id, rule);
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
