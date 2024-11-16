public class DataRule
{
    private string _successor;
    public string Successor => _successor;

    public DataRule(string successor)
    {
        _successor = successor;
    }
}