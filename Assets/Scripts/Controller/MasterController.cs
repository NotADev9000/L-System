using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class MasterController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LSystemGenerator _generator;

    [Header("Tree Location Settings")]
    [SerializeField] private Vector3 _initialGeneratorPosition = new(0, 0, 0);
    [SerializeField] private Vector3 _initialGeneratorRotation = new(-90f, 0, 0);

    [Header("UI References")]
    [SerializeField] private GameObject _masterCanvasGO;

    private MasterModel _model;
    private SymbolsController _symbolsController;
    private LineController _lineController;
    private RuleController _ruleController;
    private GeneralController _generalController;

    private void Awake()
    {
        if (_generator == null) Debug.LogError("CONTROLLER: Generator is not set in the inspector!");

        _symbolsController = GetComponent<SymbolsController>();
        _lineController = GetComponent<LineController>();
        _ruleController = GetComponent<RuleController>();
        _generalController = GetComponent<GeneralController>();
    }

    private void Start()
    {
        BigOldTestData();

        _symbolsController.SetModel(_model);
        _lineController.SetModel(_model);
        _ruleController.SetModel(_model);
        _generalController.SetModel(_model);

        _generator.SetTreeData(_model);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (_masterCanvasGO != null)
                _masterCanvasGO.SetActive(!_masterCanvasGO.activeSelf);
            else
                Debug.Log("_MasterCanvasGO is not set in the inspector!");
        }
    }

    private void BigOldTestData()
    {
        /////////////////////////////////////
        // TEST DATA                       //
        /////////////////////////////////////

        int iterations = 1;
        float angle = 25.7f;
        float angleOffset = 0.0f;
        string axiom = "K";

        SerializedDictionary<char, DataSymbol> dataSymbols = new()
        {
            {'F', new(true,
                      TurtleFunction.DrawForward,
                      new DataLine(1.0f, true, MaterialsManager.Instance.Materials[0], null),
                      new DataRule("FF", "F", 0.6f)
            )},
            {'K', new(true,
                      TurtleFunction.DrawForward,
                      new DataLine(0.5f, true, MaterialsManager.Instance.Materials[3], null),
                      new DataRule("F[+K]F[-K]+K", "F[+K]K", 0.7f)
            )},
            {'+', new(false, TurtleFunction.TurnLeft)},
            {'-', new(false, TurtleFunction.TurnRight)},
            {'[', new(false, TurtleFunction.PushState)},
            {']', new(false, TurtleFunction.PopState)}
        };

        _model = new MasterModel(iterations, angle, angleOffset, dataSymbols, axiom);

        /////////////////////////////////////
        // END TEST DATA                   //
        /////////////////////////////////////
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
