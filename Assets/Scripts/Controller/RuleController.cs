using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RuleController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Transform _ruleGroupParent;
    [SerializeField] private RectTransform _ruleGroupPrefab;

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

        // check if any symbols are variables and create UI elements for them if they have not been created already
        foreach (KeyValuePair<char, DataSymbol> kvp in model.Symbols)
        {
            if (kvp.Value.IsVariable)
                CreateRuleGroupUIElement(kvp.Key, kvp.Value.Rule);
        }
    }

    #region Model Events

    private void SubscribeToModelEvents()
    {
        MasterModel.OnSymbolRemoved += OnSymbolRemoved;
        MasterModel.OnSymbolIdUpdated += OnSymbolIdUpdated;
        MasterModel.OnIsVariableUpdated += OnIsVariableUpdated;
    }

    private void UnsubscribeFromModelEvents()
    {
        MasterModel.OnSymbolRemoved -= OnSymbolRemoved;
        MasterModel.OnSymbolIdUpdated -= OnSymbolIdUpdated;
        MasterModel.OnIsVariableUpdated -= OnIsVariableUpdated;
    }

    private void OnSymbolRemoved(char removedSymbolId)
    {
        DestroyRuleGroupUIElementWithId(removedSymbolId);
    }

    private void OnSymbolIdUpdated(char oldId, char newId)
    {
        bool existingRuleGroupUpdated = false;

        // Update line group UI element with new ID
        foreach (Transform child in _ruleGroupParent)
        {
            TMP_Text idTextLabel = child.GetComponentInChildren<TMP_Text>();
            if (idTextLabel.text == oldId.ToString())
            {
                idTextLabel.text = newId.ToString();
                existingRuleGroupUpdated = true;
                break;
            }
        }

        if (!existingRuleGroupUpdated &&
            newId != Char.MinValue &&
            _model.Symbols[newId].IsVariable)
        {
            CreateRuleGroupUIElement(newId, _model.Symbols[newId].Rule);
        }
    }

    private void OnIsVariableUpdated(char id, bool isVariable)
    {
        if (isVariable)
        {
            if (!DoesRuleGroupUIElementExist(id))
                CreateRuleGroupUIElement(id, _model.Symbols[id].Rule);
        }
        else
        {
            DestroyRuleGroupUIElementWithId(id);
        }
    }

    #endregion

    #region UI Element Management

    private void CreateRuleGroupUIElement(char dataSymbolId, DataRule dataRule = null)
    {
        dataRule ??= new(dataSymbolId.ToString());
        RectTransform ruleGroupUIElement = Instantiate(_ruleGroupPrefab, _ruleGroupParent);

        TMP_Text idTextLabel = ruleGroupUIElement.GetComponentInChildren<TMP_Text>();
        TMP_InputField[] ruleStringInputField = ruleGroupUIElement.GetComponentsInChildren<TMP_InputField>();
        TMP_InputField ruleString1InputField = ruleStringInputField[0];
        TMP_InputField ruleString2InputField = ruleStringInputField[1];

        idTextLabel.text = dataSymbolId.ToString();
        ruleString1InputField.text = dataRule.Successors[0];
        ruleString2InputField.text = dataRule.Successors[1];

        // lengthInputField.onValueChanged.AddListener((string newLengthValue) => { UI_OnLengthChanged(GetLineGroupId(ruleGroupUIElement), newLengthValue); });
    }

    private void DestroyRuleGroupUIElementWithId(char id)
    {
        foreach (Transform child in _ruleGroupParent)
        {
            if (child.GetComponentInChildren<TMP_Text>().text[0] == id)
            {
                Destroy(child.gameObject);
                Debug.Log("Rule group removed");
                break;
            }
        }
    }

    private bool DoesRuleGroupUIElementExist(char id)
    {
        foreach (Transform child in _ruleGroupParent)
        {
            if (child.GetComponentInChildren<TMP_Text>().text == id.ToString())
                return true;
        }

        return false;
    }

    #endregion
}
