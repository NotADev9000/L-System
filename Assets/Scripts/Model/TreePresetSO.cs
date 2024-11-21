using UnityEngine;

[CreateAssetMenu(fileName = "TreePreset", menuName = "L-System/TreePreset", order = 1)]
public class TreePresetSO : ScriptableObject
{
    [SerializeField] private string _presetName;
    [SerializeField] private MasterModel _treeData;
}