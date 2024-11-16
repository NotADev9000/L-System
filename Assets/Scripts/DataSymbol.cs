using System.Collections;
using System.Collections.Generic;

public class DataSymbol
{
    private char _id;
    public char Id { get { return _id; } }

    private bool _isVariable;
    public bool IsVariable { get { return _isVariable; } }

    private TurtleFunction _turtleFunction;
    public TurtleFunction TurtleFunction { get { return _turtleFunction; } }

    private DataLine _line = null;
    public DataLine Line { get { return _line; } }

    public DataSymbol(char id, bool isVariable, TurtleFunction turtleFunction, DataLine line)
    {
        _id = id;
        _isVariable = isVariable;
        _turtleFunction = turtleFunction;
        _line = line;
    }
}
