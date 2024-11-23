using UnityEngine;

public class UIDisplayController : MonoBehaviour
{
    [SerializeField] private Canvas _allUI;
    [SerializeField] private Canvas _topUI;
    [SerializeField] private Canvas _generalUI;
    [SerializeField] private Canvas _visualsUI;
    [SerializeField] private Canvas _rulesUI;
    [SerializeField] private Canvas _extraUI;

    private Canvas[] _uiDisplays;
    private bool _isTopUIShown = true;
    public bool IsTopUIShown { get { return _isTopUIShown; } }
    private bool _isAllUIShown = true;
    public bool IsAllUIShown { get { return _isAllUIShown; } }

    private void Awake()
    {
        _uiDisplays = new Canvas[] { _generalUI, _visualsUI, _rulesUI, _extraUI };
        ChangeDisplay(_generalUI);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Escape) && !_isAllUIShown)
        {
            ToggleAllUI(true);
        }
    }

    private void ChangeDisplay(Canvas display)
    {
        for (int i = 0; i < _uiDisplays.Length; i++)
        {
            _uiDisplays[i].gameObject.SetActive(_uiDisplays[i] == display);
        }
    }

    private void ToggleAllUI(bool isShown)
    {
        _allUI.gameObject.SetActive(isShown);
        _isAllUIShown = isShown;
    }

    #region UI Callbacks

    public void UI_OnToggleTopUI()
    {
        bool isShown = !_topUI.gameObject.activeSelf;
        _topUI.gameObject.SetActive(isShown);
        _isTopUIShown = isShown;
    }

    public void UI_OnHideAllUI()
    {
        ToggleAllUI(false);
    }

    public void UI_OnChangeToDisplay(Canvas ui)
    {
        ChangeDisplay(ui);
    }

    #endregion
}
