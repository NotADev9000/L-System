using System.Collections;
using System.Collections.Generic;

public class DataSymbol
{
    private bool _isVariable;
    public bool IsVariable { get { return _isVariable; } set { _isVariable = value; } }

    private TurtleFunction _turtleFunction;
    public TurtleFunction TurtleFunction { get { return _turtleFunction; } set { _turtleFunction = value; } }

    private DataLine _line = null;
    public DataLine Line { get { return _line; } }

    public DataSymbol(bool isVariable = false, TurtleFunction turtleFunction = TurtleFunction.None, DataLine line = null)
    {
        _isVariable = isVariable;
        _turtleFunction = turtleFunction;
        _line = line ?? new();
    }
}
