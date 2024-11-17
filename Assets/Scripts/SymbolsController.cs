using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SymbolsController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Transform _symbolGroupParent;
    [SerializeField] private RectTransform _symbolGroupPrefab;

    private MasterModel _model;
    // Links the UI element (symbol group) to a DataSymbol instance
    // This instance is passed to model upon any changes
    private Dictionary<RectTransform, (char currentId, DataSymbol symbol)> _symbolGroups = new();

    private const char EmptyChar = '\0';

    public void SetModel(MasterModel model)
    {
        _model = model;

        foreach (KeyValuePair<char, DataSymbol> kvp in model.Symbols)
        {
            InstantiateSymbolGroupUIElement(kvp.Key, kvp.Value);
        }
    }

    private void InstantiateSymbolGroupUIElement(char dataSymbolId = EmptyChar, DataSymbol dataSymbol = null)
    {
        dataSymbol ??= new();
        RectTransform symbolGroupUIElement = Instantiate(_symbolGroupPrefab, _symbolGroupParent);

        // Add new symbol group to dictionary
        _symbolGroups.Add(symbolGroupUIElement, (dataSymbolId, dataSymbol));

        TMP_InputField inputField = symbolGroupUIElement.GetComponentInChildren<TMP_InputField>();

        // Set UI element fields
        inputField.text = dataSymbolId.ToString();

        // Add onChange listeners to UI elements
        inputField.onValueChanged.AddListener((string newIdValue) => { UI_OnSymbolIdChanged(symbolGroupUIElement, newIdValue); });
    }

    #region UI Callbacks

    public void UI_OnAddSymbolClicked()
    {
        InstantiateSymbolGroupUIElement();
    }

    private void UI_OnSymbolIdChanged(RectTransform symbolGroup, string newIdValue)
    {
        if (_symbolGroups.TryGetValue(symbolGroup, out var pair))
        {
            char newId = newIdValue.Length > 0 ? newIdValue[0] : EmptyChar;

            if (newId == pair.currentId) return;

            // Remove old ID from model
            if (pair.currentId != EmptyChar)
            {
                _model.Symbols.Remove(pair.currentId);
            }

            // Update the ID for UI data
            _symbolGroups[symbolGroup] = (newId, pair.symbol);

            // Add new ID to model if ID isn't empty
            if (newId != EmptyChar)
            {
                _model.Symbols[newId] = pair.symbol;
            }
        }
    }

    #endregion
}
