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

    [Range(1, 10)]
    [SerializeField] private int _iterations = 1;

    private MasterModel _treeData;
    private Stack<TransformStore> _transformStack = new();
    private StringBuilder _stringBuilder = new();

    private void Start()
    {
        if (_branchPrefab == null)
        {
            Debug.LogError("Branch prefab is not set in the inspector.");
        }

        if (_treeParent == null)
        {
            Debug.LogError("Tree parent is not set in the inspector.");
        }
    }

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

    public void GenerateLSystem()
    {
        // TODO: Move tree data valdiation elsewhere
        if (_treeData == null)
        {
            Debug.LogError("FAILED TO GENERATE: No tree data supplied.");
            return;
        }
        if (_iterations < 1)
        {
            Debug.LogError("FAILED TO GENERATE: Iterations must be greater than 0.");
            return;
        }

        // Reset the transform stack
        _transformStack.Clear();

        string currentInputString = _treeData.Axiom;

        // Apply the rules to the string for the given number of iterations
        for (int i = 0; i < _iterations; i++)
        {
            currentInputString = ApplyTransformationRulesToString(currentInputString);
        }

        // Draw the L-System
        DrawLSystem(currentInputString);
    }

    private string ApplyTransformationRulesToString(string inputString)
    {
        _stringBuilder.Clear();
        Dictionary<char, DataRule> rules = _treeData.Rules;

        foreach (char c in inputString)
        {
            _stringBuilder.Append(rules.ContainsKey(c) ? rules[c].Successors[0] : c.ToString());
        }
        return _stringBuilder.ToString();
    }

    private void DrawLSystem(string inputString)
    {
        float angle = 25.7f;

        foreach (char c in inputString)
        {
            if (_treeData.Symbols.TryGetValue(c, out DataSymbol symbol))
            {
                TurtleFunction turtleFunction = symbol.TurtleFunction;

                switch (turtleFunction)
                {
                    case TurtleFunction.DrawForward:
                        DrawForward(_treeData.Symbols[c].Line);
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
                        TransformStore ts = _transformStack.Pop();
                        transform.position = ts._position;
                        transform.rotation = ts._rotation;
                        break;
                    case TurtleFunction.RotateRight:
                        transform.Rotate(Vector3.up, -angle);
                        // transform.Rotate(Vector3.right, -angle);
                        break;
                    case TurtleFunction.RotateLeft:
                        transform.Rotate(Vector3.up, angle);
                        // transform.Rotate(Vector3.right, angle);
                        break;
                    default:
                        Debug.LogWarning("No Turtle Drawing behaviour for character " + c + " in L-System string.");
                        break;
                }
            }
            else
            {
                Debug.LogWarning("No symbol found for character " + c + " in L-System string.");
            }
        }
    }

    private void DrawForward(DataLine line)
    {
        LineRenderer branch = Instantiate(_branchPrefab, transform.position, transform.rotation, _treeParent);
        Vector3 drawVector = transform.forward * line.Length;

        branch.SetPosition(0, transform.position);
        branch.SetPosition(1, transform.position + drawVector);

        if (!line.IsVisible)
            branch.gameObject.SetActive(false);

        transform.position += drawVector;
    }
}
