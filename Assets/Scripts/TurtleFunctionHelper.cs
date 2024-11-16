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

    public static string[] GetDisplayNames()
    {
        return _stringToEnum.Keys.ToArray();
    }

    public static TurtleFunction StringToFunction(string displayName)
    {
        return _stringToEnum.TryGetValue(displayName, out var function) ? function : TurtleFunction.None;
    }
}