using System;
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

    private DataRule _rule = null;
    public DataRule Rule { get { return _rule; } }

    public DataSymbol(bool isVariable = false,
    TurtleFunction turtleFunction = TurtleFunction.None,
    DataLine line = null,
    DataRule rule = null)
    {
        _isVariable = isVariable;
        _turtleFunction = turtleFunction;
        _line = line ?? new();
        _rule = rule ?? new();
    }
}
