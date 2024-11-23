using System;
using TMPro;
using UnityEngine;

public class PresetController : MonoBehaviour
{
    public event Action<int> OnPresetChanged;

    [Header("UI References")]
    [SerializeField] private TMP_Dropdown _presetsDropdown;

    private void Start()
    {
        _presetsDropdown.AddOptions(PresetManager.Instance.GetPresetNames());
    }

    public int GetSelectedPresetIndex()
    {
        return _presetsDropdown.value;
    }

    public void SelectNextPreset()
    {
        int nextIndex = _presetsDropdown.value + 1;
        if (nextIndex >= _presetsDropdown.options.Count)
            nextIndex = 0;

        _presetsDropdown.value = nextIndex;
    }

    #region UI Callbacks

    public void UI_OnPresetChanged(int index)
    {
        OnPresetChanged?.Invoke(index);
    }

    #endregion
}