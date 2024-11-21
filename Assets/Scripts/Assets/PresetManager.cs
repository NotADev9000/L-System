using System.Collections.Generic;
using UnityEngine;

public class PresetManager : MonoBehaviour
{
    [SerializeField] private TreePresetSO[] _presets;
    public TreePresetSO[] Presets => _presets;

    public static PresetManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<string> GetPresetNames()
    {
        List<string> presetNames = new List<string>();

        foreach (TreePresetSO preset in _presets)
        {
            presetNames.Add(preset.PresetName);
        }

        return presetNames;
    }
}