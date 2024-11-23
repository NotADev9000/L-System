using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using UnityEngine.XR;
using TMPro;

public class MasterController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LSystemGenerator _generator;

    [Header("Tree Location Settings")]
    [SerializeField] private Vector3 _initialGeneratorPosition = new(0, 0, 0);
    [SerializeField] private Vector3 _initialGeneratorRotation = new(-90f, 0, 0);

    private MasterModel _model;
    private PresetController _presetController;
    private SymbolsController _symbolsController;
    private LineController _lineController;
    private RuleController _ruleController;
    private GeneralController _generalController;
    private UIDisplayController _uiDisplayController;

    private TreePresetSO _currentPreset;

    #region Unity Lifecycle

    private void Awake()
    {
        if (_generator == null) Debug.LogError("CONTROLLER: Generator is not set in the inspector!");

        _presetController = GetComponent<PresetController>();
        _symbolsController = GetComponent<SymbolsController>();
        _lineController = GetComponent<LineController>();
        _ruleController = GetComponent<RuleController>();
        _generalController = GetComponent<GeneralController>();
        _uiDisplayController = GetComponent<UIDisplayController>();
    }

    private void Start()
    {
        UpdateTreeData(_presetController.GetSelectedPresetIndex());
    }

    private void OnEnable()
    {
        _presetController.OnPresetChanged += UpdateTreeData;
    }

    private void OnDisable()
    {
        _presetController.OnPresetChanged -= UpdateTreeData;
    }

    private void Update()
    {
        HandleIterationInput();
        HandlePresetInput();
    }

    private void OnDestroy()
    {
        if (_currentPreset != null)
        {
            Debug.Log($"Cleaning up preset on destroy: {_currentPreset.name}");
            Destroy(_currentPreset);
        }
    }

    #endregion

    #region Hotkeys

    private bool AreHotkeysEnabled()
    {
        return !_uiDisplayController.IsTopUIShown || !_uiDisplayController.IsAllUIShown;
    }

    private void HandleIterationInput()
    {
        for (int i = 0; i < 10; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                if (!AreHotkeysEnabled()) return;

                int newIterations = i == 0 ? 10 : i;
                _model.UpdateIterations(newIterations);
                GenerateNewTree();
            }
        }
    }

    private void HandlePresetInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!AreHotkeysEnabled()) return;

            _presetController.SelectNextPreset();
            GenerateNewTree();
        }
    }

    #endregion

    #region L-System Preparation

    private void UpdateTreeData(int index)
    {
        if (_currentPreset != null)
        {
            Debug.Log($"Destroying previous preset: {_currentPreset.name}");
            Destroy(_currentPreset);
        }

        _currentPreset = Instantiate(PresetManager.Instance.Presets[index]);
        _model = _currentPreset.TreeData;

        _symbolsController.SetModel(_model);
        _lineController.SetModel(_model);
        _ruleController.SetModel(_model);
        _generalController.SetModel(_model);

        _generator.SetTreeData(_model);
    }

    private void GenerateNewTree()
    {
        PrepareTreeGenerator();
        _generator.GenerateLSystem();
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

    #endregion

    #region UI Callbacks
    public void UI_OnGenerateClicked()
    {
        GenerateNewTree();
    }

    #endregion
}
