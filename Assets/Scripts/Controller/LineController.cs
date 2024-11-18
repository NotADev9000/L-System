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

    private MasterModel _model;

    private void OnEnable()
    {
        SubscribeToModelEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromModelEvents();
    }

    public void SetModel(MasterModel model)
    {
        _model = model;

        foreach (KeyValuePair<char, DataSymbol> kvp in model.Symbols)
        {
            if (kvp.Value.TurtleFunction == TurtleFunction.DrawForward)
                CreateLineGroupUIElement(kvp.Key, kvp.Value.Line);
        }
    }

    #region Model Events

    private void SubscribeToModelEvents()
    {
        MasterModel.OnSymbolRemoved += OnSymbolRemoved;
        MasterModel.OnSymbolIdUpdated += OnSymbolIdUpdated;
        MasterModel.OnFunctionUpdated += OnFunctionUpdated;
    }

    private void UnsubscribeFromModelEvents()
    {
        MasterModel.OnSymbolRemoved -= OnSymbolRemoved;
        MasterModel.OnSymbolIdUpdated -= OnSymbolIdUpdated;
        MasterModel.OnFunctionUpdated -= OnFunctionUpdated;
    }

    private void OnSymbolRemoved(char removedSymbolId)
    {
        DestroyLineGroupUIElementWithId(removedSymbolId);
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

    private void OnFunctionUpdated(char id, TurtleFunction newFunction)
    {
        if (newFunction == TurtleFunction.DrawForward)
        {
            CreateLineGroupUIElement(id, _model.Symbols[id].Line);
        }
        else
        {
            DestroyLineGroupUIElementWithId(id);
        }
    }

    #endregion

    #region UI Element Management

    private void CreateLineGroupUIElement(char dataSymbolId, DataLine dataLine)
    {
        RectTransform lineGroupUIElement = Instantiate(_lineGroupPrefab, _lineGroupParent);

        TMP_Text idTextLabel = lineGroupUIElement.GetComponentInChildren<TMP_Text>();
        TMP_InputField lengthInputField = lineGroupUIElement.GetComponentInChildren<TMP_InputField>();
        Toggle visibleToggle = lineGroupUIElement.GetComponentInChildren<Toggle>();
        TMP_Dropdown colorDropdown = lineGroupUIElement.GetComponentInChildren<TMP_Dropdown>();

        idTextLabel.text = dataSymbolId.ToString();
        lengthInputField.text = dataLine.Length.ToString();
        visibleToggle.isOn = dataLine.IsVisible;
        colorDropdown.AddOptions(MaterialsManager.Instance.GetMaterialNames());
        colorDropdown.value = Array.IndexOf(MaterialsManager.Instance.Materials, dataLine.Color);

        lengthInputField.onValueChanged.AddListener((string newLengthValue) => { UI_OnLengthChanged(GetLineGroupId(lineGroupUIElement), newLengthValue); });
        visibleToggle.onValueChanged.AddListener((bool newVisibleValue) => { UI_OnVisibleChanged(GetLineGroupId(lineGroupUIElement), newVisibleValue); });
        colorDropdown.onValueChanged.AddListener((int newColorValue) => { UI_OnColorChanged(GetLineGroupId(lineGroupUIElement), newColorValue); });
    }

    private void DestroyLineGroupUIElementWithId(char id)
    {
        foreach (Transform child in _lineGroupParent)
        {
            if (child.GetComponentInChildren<TMP_Text>().text[0] == id)
            {
                Destroy(child.gameObject);
                Debug.Log("Line group removed");
                break;
            }
        }
    }

    private char GetLineGroupId(RectTransform lineGroupUIElement)
    {
        return lineGroupUIElement.GetComponentInChildren<TMP_Text>().text[0];
    }

    #endregion

    #region UI Callbacks

    private void UI_OnLengthChanged(char id, string newLengthValue)
    {
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

    private void UI_OnVisibleChanged(char id, bool isVisible)
    {
        _model.Symbols[id].Line.UpdateVisibility(isVisible);
    }

    private void UI_OnColorChanged(char id, int colorDropdownIndex)
    {
        _model.Symbols[id].Line.UpdateColor(MaterialsManager.Instance.Materials[colorDropdownIndex]);
    }

    #endregion
}
