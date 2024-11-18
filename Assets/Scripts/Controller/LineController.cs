using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LineController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Transform _lineGroupParent;
    [SerializeField] private RectTransform _lineGroupPrefab;
    [SerializeField] private TMP_Text _lineWarningText;

    private MasterModel _model;

    private void Awake()
    {
        SubscribeToModelEvents();
    }

    public void SetModel(MasterModel model)
    {
        _model = model;
        bool thereIsASymbolWithDrawFunc = false;

        foreach (KeyValuePair<char, DataSymbol> kvp in model.Symbols)
        {
            if (kvp.Value.TurtleFunction == TurtleFunction.DrawForward)
            {
                CreateLineGroupUIElement(kvp.Key, kvp.Value.Line);
                thereIsASymbolWithDrawFunc = true;
            }
        }

        if (!thereIsASymbolWithDrawFunc) _lineWarningText.gameObject.SetActive(true);
    }

    private char GetLineGroupId(RectTransform lineGroupUIElement)
    {
        return lineGroupUIElement.GetComponentInChildren<TMP_Text>().text[0];
    }

    private void SubscribeToModelEvents()
    {
        MasterModel.OnSymbolRemoved += OnSymbolRemoved;
        MasterModel.OnSymbolIdUpdated += OnSymbolIdUpdated;
    }

    private void OnSymbolRemoved(char removedSymbolId)
    {
        // Remove line group UI element with corresponding ID
        foreach (Transform child in _lineGroupParent)
        {
            if (child.GetComponentInChildren<TMP_Text>().text == removedSymbolId.ToString())
            {
                Destroy(child.gameObject);
                Debug.Log("Line group removed");
                break;
            }
        }
    }

    private void OnSymbolIdUpdated(char oldId, char newId)
    {
        // Update line group UI element with new ID
        foreach (Transform child in _lineGroupParent)
        {
            TMP_Text idTextLabel = child.GetComponentInChildren<TMP_Text>();
            if (idTextLabel.text == oldId.ToString())
            {
                idTextLabel.text = newId.ToString();
                Debug.Log("Line group ID updated");
                break;
            }
        }
    }

    private void CreateLineGroupUIElement(char dataSymbolId, DataLine dataLine)
    {
        RectTransform lineGroupUIElement = Instantiate(_lineGroupPrefab, _lineGroupParent);

        TMP_Text idTextLabel = lineGroupUIElement.GetComponentInChildren<TMP_Text>();
        TMP_InputField lengthInputField = lineGroupUIElement.GetComponentInChildren<TMP_InputField>();
        Toggle visibleToggle = lineGroupUIElement.GetComponentInChildren<Toggle>();
        TMP_Dropdown colorDropdown = lineGroupUIElement.GetComponentInChildren<TMP_Dropdown>();
        // TMP_InputField leafPrefabInputField = lineGroupUIElement.GetComponentInChildren<TMP_InputField>();

        idTextLabel.text = dataSymbolId.ToString();
        lengthInputField.text = dataLine.Length.ToString();
        visibleToggle.isOn = dataLine.IsVisible;
        colorDropdown.AddOptions(MaterialsManager.Instance.GetMaterialNames());
        colorDropdown.value = Array.IndexOf(MaterialsManager.Instance.Materials, dataLine.Color);
        // leafPrefabInputField.text = dataSymbol.LeafPrefab.ToString();

        lengthInputField.onValueChanged.AddListener((string newLengthValue) => { UI_OnLengthChanged(lineGroupUIElement, newLengthValue); });
        visibleToggle.onValueChanged.AddListener((bool newVisibleValue) => { UI_OnVisibleChanged(lineGroupUIElement, newVisibleValue); });
        colorDropdown.onValueChanged.AddListener((int newColorValue) => { UI_OnColorChanged(lineGroupUIElement, newColorValue); });
        // leafPrefabInputField.onValueChanged.AddListener((string newLeafPrefabValue) => { UI_OnLeafPrefabChanged(lineGroupUIElement, newLeafPrefabValue); });
    }

    #region UI Callbacks

    private void UI_OnLengthChanged(RectTransform lineGroupUIElement, string newLengthValue)
    {
        char id = GetLineGroupId(lineGroupUIElement);
        try
        {
            float newLength = Math.Abs(Convert.ToSingle(newLengthValue));
            _model.Symbols[id].Line.UpdateLength(newLength);
        }
        catch (FormatException)
        {
            Debug.LogWarning("Length value must be a number.");
            return;
        }
    }

    private void UI_OnVisibleChanged(RectTransform lineGroupUIElement, bool isVisible)
    {
        char id = GetLineGroupId(lineGroupUIElement);
        _model.Symbols[id].Line.UpdateVisibility(isVisible);
    }

    private void UI_OnColorChanged(RectTransform lineGroupUIElement, int colorDropdownIndex)
    {
        char id = GetLineGroupId(lineGroupUIElement);
        _model.Symbols[id].Line.UpdateColor(MaterialsManager.Instance.Materials[colorDropdownIndex]);
    }

    #endregion
}
