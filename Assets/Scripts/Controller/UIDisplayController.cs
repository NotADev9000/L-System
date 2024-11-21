using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDisplayController : MonoBehaviour
{
    [SerializeField] private Canvas[] _uiDisplays;
    [SerializeField] private int _startingDisplayIndex = 0;
    [SerializeField] private Canvas _allUI;
    [SerializeField] private Canvas _topUI;

    private int _uiDisplayIndex = 0;
    private bool _isAllUIShown = true;

    private void Awake()
    {
        if (_uiDisplays.Length == 0)
        {
            Debug.LogError("No UI displays set in the inspector.");
        }
        else
        {
            _uiDisplayIndex = Mathf.Clamp(_startingDisplayIndex, 0, _uiDisplays.Length - 1);
            SetActiveDisplay();
        }
    }

    private void Update()
    {
        int indexChange = Input.GetKeyDown(KeyCode.RightArrow) ? 1 : Input.GetKeyDown(KeyCode.LeftArrow) ? -1 : 0;
        if (indexChange != 0)
            ChangeDisplayIndex(indexChange);

        if (Input.GetMouseButtonDown(0) && !_isAllUIShown)
        {
            ToggleAllUI(true);
        }
    }

    private void ChangeDisplayIndex(int indexChange)
    {
        _uiDisplayIndex += indexChange;
        _uiDisplayIndex = (_uiDisplayIndex + _uiDisplays.Length) % _uiDisplays.Length;
        SetActiveDisplay();
    }

    private void SetActiveDisplay()
    {
        for (int i = 0; i < _uiDisplays.Length; i++)
        {
            _uiDisplays[i].gameObject.SetActive(i == _uiDisplayIndex);
        }
    }

    private void ToggleAllUI(bool isShown)
    {
        _allUI.gameObject.SetActive(isShown);
        _isAllUIShown = isShown;
    }

    #region UI Callbacks

    public void ToggleTopUI()
    {
        _topUI.gameObject.SetActive(!_topUI.gameObject.activeSelf);
    }

    public void HideAllUI()
    {
        ToggleAllUI(false);
    }

    #endregion
}
