public class DataRule
{
    private string[] _successors = new string[2];
    public string[] Successors => _successors;

    private float _stochasticChance;
    public float StochasticChance => _stochasticChance;

    public DataRule(string successor1, string successor2 = "", float stochasticChance = 1f)
    {
        _successors[0] = successor1;
        _successors[1] = successor2;
        _stochasticChance = stochasticChance;
    }
}
