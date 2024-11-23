using UnityEngine;
using UnityEngine.UI;

public class MasterController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LSystemGenerator _generator;

    [Header("Camera Settings")]
    [SerializeField] private Transform _cameraPivot;
    [SerializeField] private Camera _camera;
    [SerializeField] private float _cameraMoveSpeed = 25f;
    [SerializeField] private float _cameraGroundLimit = 0f;
    [Tooltip("The closest distance the camera can be to the camera pivot")]
    [SerializeField] private float _cameraZoomLimit = 5f;
    [SerializeField] private float _cameraAutoRotateSpeed = 15f;

    [Header("Tree Location Settings")]
    [SerializeField] private Vector3 _initialGeneratorPosition = new(0, 0, 0);
    [SerializeField] private Vector3 _initialGeneratorRotation = new(-90f, 0, 0);

    [Header("UI Elements")]
    [SerializeField] private Toggle _animateToggle;
    [SerializeField] private Toggle _autoRotateToggle;

    [Header("Visuals")]
    [SerializeField] private GameObject _groundPlane;

    private MasterModel _model;
    private PresetController _presetController;
    private SymbolsController _symbolsController;
    private LineController _lineController;
    private RuleController _ruleController;
    private GeneralController _generalController;
    private UIDisplayController _uiDisplayController;

    private TreePresetSO _currentPreset;

    private Vector3 _cameraInitialPosition;
    private Vector3 _cameraPivotInitialPosition;
    private Quaternion _cameraPivotInitialRotation;

    private bool _autoRotate = false;

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

        _cameraPivotInitialPosition = _cameraPivot.transform.position;
        _cameraPivotInitialRotation = _cameraPivot.transform.rotation;
        _cameraInitialPosition = _camera.transform.position;
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
        HandleCameraMovement();
        HandleIterationInput();
        HandlePresetInput();
        HandleAngleInput();
        HandleAngleOffsetInput();
        HandleAutoRotateInput();
        HandleAnimateInput();
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

    private void HandleCameraMovement()
    {
        if (_autoRotate) HandleAutoRotationCameraMovement();

        if (!AreHotkeysEnabled()) return;

        HandleVerticalCameraMovement();
        HandleHorizontalCameraMovement();
        HandleZoomCameraMovement();
        if (!_autoRotate) HandleRotationCameraMovement();
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

    private void HandleAngleInput()
    {
        int angleChange = Input.GetKeyDown(KeyCode.Minus) ? -1 : Input.GetKeyDown(KeyCode.Equals) ? 1 : 0;
        if (AreHotkeysEnabled() && angleChange != 0)
        {
            _model.UpdateAngleByAmount(angleChange);
            GenerateNewTree();
        }
    }

    private void HandleAngleOffsetInput()
    {
        int angleChange = Input.GetKeyDown(KeyCode.LeftBracket) ? -1 : Input.GetKeyDown(KeyCode.RightBracket) ? 1 : 0;
        if (AreHotkeysEnabled() && angleChange != 0)
        {
            _model.UpdateAngleOffsetByAmount(angleChange);
            GenerateNewTree();
        }
    }

    private void HandleAutoRotateInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _autoRotate = !_autoRotate;
            _autoRotateToggle.isOn = _autoRotate;
        }
    }

    private void HandleAnimateInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            _generator.DoAnimate = !_generator.DoAnimate;
            _animateToggle.isOn = _generator.DoAnimate;
            GenerateNewTree();
        }
    }

    #endregion

    #region Camera Movement

    private void HandleVerticalCameraMovement()
    {
        Vector3 direction = _cameraPivot.transform.up;
        direction *= Input.GetKey(KeyCode.DownArrow) ? -1 : Input.GetKey(KeyCode.UpArrow) ? 1 : 0;
        _cameraPivot.transform.position += _cameraMoveSpeed * Time.deltaTime * direction;

        if (_groundPlane.activeSelf && _cameraPivot.transform.position.y < _cameraGroundLimit)
            _cameraPivot.transform.position = new Vector3(_cameraPivot.transform.position.x, _cameraGroundLimit, _cameraPivot.transform.position.z);
    }

    private void HandleHorizontalCameraMovement()
    {
        Vector3 direction = Vector3.right;
        direction *= Input.GetKey(KeyCode.LeftArrow) ? -1 : Input.GetKey(KeyCode.RightArrow) ? 1 : 0;
        _cameraPivot.transform.position += _cameraMoveSpeed * Time.deltaTime * direction;
    }

    private void HandleZoomCameraMovement()
    {
        Vector3 direction = _camera.transform.forward;
        direction *= Input.GetKey(KeyCode.S) ? -1 : Input.GetKey(KeyCode.W) && !IsAtZoomLimit() ? 1 : 0;
        _camera.transform.position += _cameraMoveSpeed * Time.deltaTime * direction;
    }

    private bool IsAtZoomLimit()
    {
        Vector3 cameraPos = _camera.transform.position;
        Vector3 pivotPos = _cameraPivot.transform.position;

        // Only check for distance on the x and z axis
        cameraPos.y = 0;
        pivotPos.y = 0;

        float horizontalDistance = Vector3.Distance(cameraPos, pivotPos);
        return horizontalDistance < _cameraZoomLimit;
    }

    private void HandleRotationCameraMovement()
    {
        RotateCameraOnY(Input.GetKey(KeyCode.D) ? -1 : Input.GetKey(KeyCode.A) ? 1 : 0);
    }

    private void HandleAutoRotationCameraMovement()
    {
        RotateCameraOnY(Time.deltaTime * -_cameraAutoRotateSpeed);
    }

    private void RotateCameraOnY(float angleChange)
    {
        Vector3 rotation = _cameraPivot.transform.rotation.eulerAngles;
        rotation.y += angleChange;
        _cameraPivot.transform.rotation = Quaternion.Euler(rotation);
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

    public void UI_OnRotateToggleChanged(bool isOn)
    {
        _autoRotate = isOn;
    }

    public void UI_OnAnimateToggleChanged(bool isOn)
    {
        _generator.DoAnimate = isOn;
    }

    public void UI_OnToggleGroundPlane()
    {
        _groundPlane.SetActive(!_groundPlane.activeSelf);
    }

    public void UI_OnResetCameraPosition()
    {
        _cameraPivot.transform.position = _cameraPivotInitialPosition;
        _cameraPivot.transform.rotation = _cameraPivotInitialRotation;
        _camera.transform.position = _cameraInitialPosition;
    }

    #endregion
}
