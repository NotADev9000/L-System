using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LSystemGenerator _generator;

    [Header("Tree Location Settings")]
    [SerializeField] private Vector3 _initialGeneratorPosition = new(0, 0, 0);
    [SerializeField] private Vector3 _initialGeneratorRotation = new(-90f, 0, 0);

    private MasterModel _model;

    private void Awake()
    {
        if (_generator == null) Debug.LogError("CONTROLLER: LSystemGenerator is not set in the inspector!");

        /////////////////////////////////////
        // TEST DATA                       //
        /////////////////////////////////////

        string axiom = "K";

        List<DataSymbol> dataSymbols = new List<DataSymbol>
        {
            new('F', true, TurtleFunction.DrawForward, new DataLine(1.0f, true, Color.green, null)),
            new('K', true, TurtleFunction.DrawForward, new DataLine(0.5f, true, Color.red, null)),
            new('+', false, TurtleFunction.RotateRight, null),
            new('-', false, TurtleFunction.RotateLeft, null),
            new('[', false, TurtleFunction.PushState, null),
            new(']', false, TurtleFunction.PopState, null)
        };

        Dictionary<char, DataRule> rulesDict = new Dictionary<char, DataRule>
        {
            { 'F', new DataRule("FF") },
            { 'K', new DataRule("F[+K]F[-K]+K") }
        };

        _model = new MasterModel(axiom, dataSymbols, rulesDict);

        /////////////////////////////////////
        // END TEST DATA                   //
        /////////////////////////////////////
    }

    private void Start()
    {
        _generator.SetTreeData(_model);
    }

    private void PrepareTreeGenerator()
    {
        _generator.DestroyTree();
        ResetGeneratorTransform();
    }

    private void ResetGeneratorTransform()
    {
        _generator.transform.position = _initialGeneratorPosition;
        _generator.transform.rotation = Quaternion.Euler(_initialGeneratorRotation);
    }

    #region UI Callbacks
    public void UI_OnGenerateClicked()
    {
        PrepareTreeGenerator();
        _generator.GenerateLSystem();

    }

    #endregion
}
