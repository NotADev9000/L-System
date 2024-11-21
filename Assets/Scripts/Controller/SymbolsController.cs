using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SymbolsController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Transform _symbolGroupParent;
    [SerializeField] private RectTransform _symbolGroupPrefab;

    private MasterModel _model;

    // Links the UI element (symbol group) to a DataSymbol instance
    // This instance is also referenced by the MasterModel
    private Dictionary<RectTransform, (char currentId, DataSymbol symbol)> _symbolGroups = new();

    public void SetModel(MasterModel model)
    {
        _model = model;
        ClearAllUIElements();
        CreateAllUIElements();
    }

    private void CreateAllUIElements()
    {
        foreach (KeyValuePair<char, DataSymbol> kvp in _model.Symbols)
        {
            CreateSymbolGroupUIElement(kvp.Key, kvp.Value);
        }
    }

    private void ClearAllUIElements()
    {
        foreach (RectTransform symbolGroup in _symbolGroups.Keys)
        {
            Destroy(symbolGroup.gameObject);
        }
        _symbolGroups.Clear();
    }

    private void CreateSymbolGroupUIElement(char dataSymbolId = Char.MinValue, DataSymbol dataSymbol = null)
    {
        dataSymbol ??= new();
        RectTransform symbolGroupUIElement = Instantiate(_symbolGroupPrefab, _symbolGroupParent);

        // Add new symbol group to dictionary
        _symbolGroups.Add(symbolGroupUIElement, (dataSymbolId, dataSymbol));

        TMP_InputField idInputField = symbolGroupUIElement.GetComponentInChildren<TMP_InputField>();
        TMP_Dropdown functionDropdown = symbolGroupUIElement.GetComponentInChildren<TMP_Dropdown>();
        Toggle variableToggle = symbolGroupUIElement.GetComponentInChildren<Toggle>();

        // Set UI element fields
        idInputField.text = dataSymbolId.ToString();
        functionDropdown.AddOptions(TurtleFunctionHelper.GetDisplayNames());
        functionDropdown.value = (int)dataSymbol.TurtleFunction;
        variableToggle.isOn = dataSymbol.IsVariable;

        // Add onChange listeners to UI elements
        idInputField.onValueChanged.AddListener((string newIdValue) => { UI_OnSymbolIdChanged(symbolGroupUIElement, newIdValue); });
        functionDropdown.onValueChanged.AddListener((int newFunctionValue) => { UI_OnSymbolFunctionChanged(symbolGroupUIElement, newFunctionValue); });
        variableToggle.onValueChanged.AddListener((bool isVariable) => { UI_OnVariableToggleChanged(symbolGroupUIElement, isVariable); });
    }

    private void RemoveSymbolGroupUIElement(RectTransform symbolGroup)
    {
        char id = _symbolGroups[symbolGroup].currentId;
        // Remove symbol from model if it exists
        _model.RemoveSymbol(id);
        // Remove symbol group from the link
        _symbolGroups.Remove(symbolGroup);
        // Destroy the UI element
        Destroy(symbolGroup.gameObject);
    }

    #region UI Callbacks

    public void UI_OnAddSymbolClicked()
    {
        CreateSymbolGroupUIElement();
    }

    public void UI_OnRemoveSymbolClicked()
    {
        if (_symbolGroups.Count > 0)
        {
            RectTransform lastSymbolGroup = _symbolGroups.Keys.Last();
            RemoveSymbolGroupUIElement(lastSymbolGroup);
        }
    }

    private void UI_OnSymbolIdChanged(RectTransform symbolGroup, string newIdValue)
    {
        if (_symbolGroups.TryGetValue(symbolGroup, out var pair))
        {
            char newId = newIdValue.Length > 0 ? newIdValue[0] : Char.MinValue;
            if (newId == pair.currentId) return;

            _model.UpdateSymbolId(pair.currentId, newId, pair.symbol);
            // Update the ID for UI data
            _symbolGroups[symbolGroup] = (newId, pair.symbol);
        }
    }

    private void UI_OnSymbolFunctionChanged(RectTransform symbolGroup, int newFunctionValue)
    {
        if (_symbolGroups.TryGetValue(symbolGroup, out var pair))
        {
            TurtleFunction newFunction = (TurtleFunction)newFunctionValue;
            if (newFunction == pair.symbol.TurtleFunction) return;

            _model.UpdateSymbolFunction(pair.currentId, newFunction);
            // Update the function for UI data
            pair.symbol.TurtleFunction = newFunction;
        }
    }

    private void UI_OnVariableToggleChanged(RectTransform symbolGroup, bool isVariable)
    {
        if (_symbolGroups.TryGetValue(symbolGroup, out var pair))
        {
            _model.UpdateSymbolIsVariable(pair.currentId, isVariable);
            // Update the variable for UI data
            pair.symbol.IsVariable = isVariable;
        }
    }

    #endregion
}
