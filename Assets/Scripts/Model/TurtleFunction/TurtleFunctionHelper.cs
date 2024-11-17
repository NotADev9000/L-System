using System.Collections.Generic;
using System.Linq;

public static class TurtleFunctionHelper
{
    private static readonly Dictionary<string, TurtleFunction> _stringToEnum = new()
    {
        { "None", TurtleFunction.None },
        { "Draw Forward", TurtleFunction.DrawForward },
        { "Rotate Right", TurtleFunction.RotateRight },
        { "Rotate Left", TurtleFunction.RotateLeft },
        { "Push State", TurtleFunction.PushState },
        { "Pop State", TurtleFunction.PopState }
    };

    public static List<string> GetDisplayNames()
    {
        return _stringToEnum.Keys.ToList();
    }

    public static TurtleFunction StringToFunction(string displayName)
    {
        return _stringToEnum.TryGetValue(displayName, out var function) ? function : TurtleFunction.None;
    }

    public static string FunctionToString(TurtleFunction function)
    {
        return _stringToEnum.First(entry => entry.Value == function).Key;
    }
}