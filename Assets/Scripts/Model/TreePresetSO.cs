using UnityEngine;

[CreateAssetMenu(fileName = "TreePreset", menuName = "L-System/TreePreset", order = 1)]
public class TreePresetSO : ScriptableObject
{
    [SerializeField] private string _presetName;
    public string PresetName => _presetName;
    [SerializeField] private MasterModel _treeData;
    public MasterModel TreeData => _treeData;
}
