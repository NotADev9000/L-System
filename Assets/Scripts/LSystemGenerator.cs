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
    private string _axiom = "0";

    [Range(1, 10)]
    [SerializeField] private int _iterations = 1;
    private Dictionary<char, string> _rules;
    private Stack<TransformStore> _transformStack = new();
    private StringBuilder _stringBuilder = new();

    private void Awake()
    {
        _rules = new Dictionary<char, string> {
            { '0', "1[0]0" },
            { '1', "11" }
        };
    }

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _branchPrefab != null)
        {
            if (_iterations < 1)
            {
                Debug.LogError("Iterations must be greater than 0.");
                return;
            }
            GenerateLSystem();
        }
    }

    private void GenerateLSystem()
    {
        string currentInputString = _axiom;
        Debug.Log("iteration 0: " + currentInputString);

        // Apply the rules to the string for the given number of iterations
        for (int i = 0; i < _iterations; i++)
        {
            currentInputString = ApplyTransformationRulesToString(currentInputString);
            Debug.Log("iteration " + (i + 1) + ": " + currentInputString);
        }

        // Draw the L-System
        DrawLSystem(currentInputString);
    }

    private string ApplyTransformationRulesToString(string inputString)
    {
        _stringBuilder.Clear();

        foreach (char c in inputString)
        {
            _stringBuilder.Append(_rules.ContainsKey(c) ? _rules[c] : c.ToString());
        }
        return _stringBuilder.ToString();
    }

    private void DrawLSystem(string inputString)
    {
        float angle = 45f;

        foreach (char c in inputString)
        {
            switch (c)
            {
                case '0':
                    DrawForward(0.4f);
                    break;
                case '1':
                    DrawForward(1f);
                    break;
                case '[':
                    _transformStack.Push(new TransformStore { _position = transform.position, _rotation = transform.rotation });

                    transform.Rotate(Vector3.up, -angle);
                    break;
                case ']':
                    TransformStore ts = _transformStack.Pop();
                    transform.position = ts._position;
                    transform.rotation = ts._rotation;

                    transform.Rotate(Vector3.up, angle);
                    break;
                // case '+':
                //     transform.Rotate(Vector3.up, angle);
                //     break;
                // case '-':
                //     transform.Rotate(Vector3.up, -angle);
                //     break;
                default:
                    Debug.LogError("Invalid character in L-System string: " + c);
                    break;
            }
        }
    }

    private void DrawForward(float length)
    {
        LineRenderer branch = Instantiate(_branchPrefab, transform.position, transform.rotation, _treeParent);
        Vector3 drawVector = transform.forward * length;

        branch.SetPosition(0, transform.position);
        branch.SetPosition(1, transform.position + drawVector);

        transform.position += drawVector;
    }
}
