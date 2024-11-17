using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SymbolsController : MonoBehaviour
{
    [SerializeField] private Transform _symbolGroupParent;
    [SerializeField] private RectTransform _symbolGroupPrefab;

    private MasterModel _model;
    private Dictionary<RectTransform, (char currentId, DataSymbol symbol)> _symbolGroups = new();

    private const char EmptyChar = '\0';

    public void SetModel(MasterModel model)
    {
        _model = model;

        foreach (KeyValuePair<char, DataSymbol> kvp in model.Symbols)
        {
            RectTransform symbolGroup = Instantiate(_symbolGroupPrefab, _symbolGroupParent);

            TMP_InputField inputField = symbolGroup.GetComponentInChildren<TMP_InputField>();
            inputField.text = kvp.Key.ToString();
            inputField.onValueChanged.AddListener((string newIdValue) => { OnSymbolIdChanged(symbolGroup, newIdValue); });

            _symbolGroups.Add(symbolGroup, (kvp.Key, kvp.Value));
        }
    }

    public void UI_OnAddSymbolClicked()
    {
        RectTransform symbolGroup = Instantiate(_symbolGroupPrefab, _symbolGroupParent);

        DataSymbol symbol = new(false, TurtleFunction.None, null);
        _symbolGroups.Add(symbolGroup, ('\0', symbol));

        TMP_InputField inputField = symbolGroup.GetComponentInChildren<TMP_InputField>();
        inputField.onValueChanged.AddListener((string newIdValue) => { OnSymbolIdChanged(symbolGroup, newIdValue); });
    }

    private void OnSymbolIdChanged(RectTransform symbolGroup, string newIdValue)
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

            // Add new ID to model if it isn't empty
            if (newId != EmptyChar)
            {
                _model.Symbols[newId] = pair.symbol;
            }
        }

        Debug.Log(_model.Symbols);
        Debug.Log(_model.Rules);
    }
}
