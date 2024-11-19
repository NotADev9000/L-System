using UnityEngine;

public class DataRule
{
    private string[] _successors = new string[2];
    public string Successor1 { get { return _successors[0]; } }
    public string Successor2 { get { return _successors[1]; } }

    private float _stochasticChance;
    public float StochasticChance => _stochasticChance;

    public DataRule(string successor1 = "", string successor2 = "", float stochasticChance = 0f)
    {
        _successors[0] = successor1;
        _successors[1] = successor2;
        SetStochasticChance(stochasticChance);
    }

    public void UpdateSuccessor(int successorNum, string newSuccessor)
    {
        _successors[successorNum - 1] = newSuccessor;
    }

    public void SetStochasticChance(float newStochasticChance)
    {
        _stochasticChance = Mathf.Clamp(newStochasticChance, 0f, 1f);
    }
}
