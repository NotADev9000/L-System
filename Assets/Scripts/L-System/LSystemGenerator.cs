using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TransformStore
{
    public Vector3 _position;
    public Quaternion _rotation;
}

public class LSystemGenerator : MonoBehaviour
{
    [SerializeField] private Transform _treeParent;
    [SerializeField] private LineRenderer _branchPrefab;

    private MasterModel _treeData;

    // L-System variables
    private Stack<TransformStore> _transformStack = new();
    private StringBuilder _stringBuilder = new();

    // Drawing variables
    private char _lastSymbolIdDrawn;
    private LineRenderer _currentRunningBranch;
    private int _currentRunningBranchIndex;

    public bool DoAnimate { get; set; }

    private void Awake()
    {
        if (_branchPrefab == null)
            Debug.LogError("Branch prefab is not set in the inspector.");
        if (_treeParent == null)
            Debug.LogError("Tree parent is not set in the inspector.");
    }

    #region Tree Prep

    public void SetTreeData(MasterModel treeData)
    {
        _treeData = treeData;
    }

    public void DestroyTree()
    {
        foreach (Transform child in _treeParent)
        {
            Destroy(child.gameObject);
        }
    }

    #endregion

    #region Generation

    public void GenerateLSystem()
    {
        StopAllCoroutines();

        // Reset the transform stack
        _transformStack.Clear();

        string currentInputString = _treeData.Axiom;

        // Apply the rules to the string for the given number of iterations
        for (int i = 0; i < _treeData.Iterations; i++)
        {
            currentInputString = ApplyTransformationRulesToString(currentInputString);
        }

        // Draw the L-System
        StartCoroutine(DrawLSystem(currentInputString));
    }

    #endregion

    #region Applying Rules

    private string ApplyTransformationRulesToString(string inputString)
    {
        _stringBuilder.Clear();
        Dictionary<char, DataSymbol> symbols = _treeData.Symbols;

        foreach (char c in inputString)
        {
            _stringBuilder.Append(symbols.ContainsKey(c) && symbols[c].IsVariable ? CalculateSuccessorString(symbols[c].Rule) : c);
        }
        return _stringBuilder.ToString();
    }

    private string CalculateSuccessorString(DataRule rule)
    {
        if (rule.StochasticChance == 0) return rule.Successor1;
        return rule.Successor2 != null && UnityEngine.Random.value < rule.StochasticChance ? rule.Successor2 : rule.Successor1;
    }

    #endregion

    #region Turtle Drawing

    private IEnumerator DrawLSystem(string inputString)
    {
        _currentRunningBranch = null;

        foreach (char c in inputString)
        {
            if (_treeData.Symbols.TryGetValue(c, out DataSymbol symbol))
            {
                TurtleFunction turtleFunction = symbol.TurtleFunction;

                switch (turtleFunction)
                {
                    case TurtleFunction.DrawForward:
                        DrawForward(symbol.Line, c);
                        if (DoAnimate) yield return null;
                        break;
                    case TurtleFunction.PushState:
                        _transformStack.Push(new TransformStore { _position = transform.position, _rotation = transform.rotation });
                        break;
                    case TurtleFunction.PopState:
                        if (_transformStack.Count == 0)
                        {
                            Debug.LogWarning("Tried to pop an empty transform stack!");
                            break;
                        }
                        _currentRunningBranch = null;
                        TransformStore ts = _transformStack.Pop();
                        transform.position = ts._position;
                        transform.rotation = ts._rotation;
                        break;
                    case TurtleFunction.TurnLeft:
                        RotateTurtle(Vector3.up, false);
                        break;
                    case TurtleFunction.TurnRight:
                        RotateTurtle(Vector3.up, true);
                        break;
                    case TurtleFunction.PitchDown:
                        RotateTurtle(Vector3.right, true);
                        break;
                    case TurtleFunction.PitchUp:
                        RotateTurtle(Vector3.right, false);
                        break;
                    case TurtleFunction.RollLeft:
                        RotateTurtle(Vector3.forward, true);
                        break;
                    case TurtleFunction.RollRight:
                        RotateTurtle(Vector3.forward, false);
                        break;
                    case TurtleFunction.Turn180:
                        RotateTurtle(Vector3.up, true);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void DrawForward(DataLine line, char symbolId)
    {
        if (_currentRunningBranch == null || _lastSymbolIdDrawn != symbolId)
        {
            _currentRunningBranch = Instantiate(_branchPrefab, transform.position, transform.rotation, _treeParent);
            if (!line.IsVisible) _currentRunningBranch.gameObject.SetActive(false);
            _currentRunningBranch.SetPosition(0, transform.position);
            _currentRunningBranch.material = line.Color;
            _currentRunningBranchIndex = 1;
            _lastSymbolIdDrawn = symbolId;
        }

        Vector3 drawVector = transform.forward * line.Length;

        _currentRunningBranch.positionCount = _currentRunningBranchIndex + 1;
        _currentRunningBranch.SetPosition(_currentRunningBranchIndex, transform.position + drawVector);
        _currentRunningBranchIndex++;

        transform.position += drawVector;
    }

    private void RotateTurtle(Vector3 axis, bool isPositiveRotation)
    {
        float angle = _treeData.Angle * (isPositiveRotation ? 1 : -1);
        angle = ApplyAngleOffset(angle);
        transform.Rotate(axis, angle);
    }

    private float ApplyAngleOffset(float angle)
    {
        float randomOffset = UnityEngine.Random.Range(-_treeData.AngleOffset, _treeData.AngleOffset);
        return angle + randomOffset;
    }

    #endregion
}
