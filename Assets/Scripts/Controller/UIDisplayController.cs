using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDisplayController : MonoBehaviour
{
    [SerializeField] private Canvas[] _uiDisplays;
    [SerializeField] private int _startingDisplayIndex = 0;

    private int _uiDisplayIndex = 0;

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
}
