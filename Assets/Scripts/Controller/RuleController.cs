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
    [SerializeField] private TMP_InputField _axiomInputField;

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
        ClearAllUIElements();
        CreateAllUIElements();
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

    private void ClearAllUIElements()
    {
        foreach (Transform child in _ruleGroupParent)
        {
            Destroy(child.gameObject);
        }

        _axiomInputField.SetTextWithoutNotify(string.Empty);
    }

    private void CreateAllUIElements()
    {
        foreach (KeyValuePair<char, DataSymbol> kvp in _model.Symbols)
        {
            if (kvp.Value.IsVariable)
                CreateRuleGroupUIElement(kvp.Key, kvp.Value.Rule);
        }

        _axiomInputField.SetTextWithoutNotify(_model.Axiom);
    }

    private void CreateRuleGroupUIElement(char dataSymbolId, DataRule dataRule = null)
    {
        dataRule ??= new(dataSymbolId.ToString());
        RectTransform ruleGroupUIElement = Instantiate(_ruleGroupPrefab, _ruleGroupParent);

        TMP_Text idTextLabel = ruleGroupUIElement.GetComponentInChildren<TMP_Text>();
        TMP_InputField[] ruleGroupInputFields = ruleGroupUIElement.GetComponentsInChildren<TMP_InputField>();
        TMP_InputField ruleString1InputField = ruleGroupInputFields[0];
        TMP_InputField stochasticChanceInputField = ruleGroupInputFields[1];
        TMP_InputField ruleString2InputField = ruleGroupInputFields[2];

        idTextLabel.text = dataSymbolId.ToString();
        ruleString1InputField.text = dataRule.Successor1;
        ruleString2InputField.text = dataRule.Successor2;
        stochasticChanceInputField.text = ConvertStochasticChanceToDisplayString(dataRule.StochasticChance);

        ruleString1InputField.onValueChanged.AddListener((string newRuleStringValue) =>
        {
            UI_OnRuleString1Changed(GetRuleGroupId(ruleGroupUIElement), newRuleStringValue);
        });
        ruleString2InputField.onValueChanged.AddListener((string newRuleStringValue) =>
        {
            UI_OnRuleString2Changed(GetRuleGroupId(ruleGroupUIElement), newRuleStringValue);
        });
        stochasticChanceInputField.onValueChanged.AddListener((string newChanceValue) =>
        {
            UI_OnStochasticChanceChanged(GetRuleGroupId(ruleGroupUIElement), newChanceValue, stochasticChanceInputField);
        });
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

    private char GetRuleGroupId(RectTransform ruleGroupUIElement)
    {
        return ruleGroupUIElement.GetComponentInChildren<TMP_Text>().text[0];
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

    private string ConvertStochasticChanceToDisplayString(float stochasticChance)
    {
        return (stochasticChance * 100f).ToString();
    }

    #endregion

    #region UI Callbacks

    private void UI_OnRuleString1Changed(char id, string newRuleString1Value)
    {
        _model.Symbols[id].Rule.UpdateSuccessor(1, newRuleString1Value);
    }

    private void UI_OnRuleString2Changed(char id, string newRuleString2Value)
    {
        _model.Symbols[id].Rule.UpdateSuccessor(2, newRuleString2Value);
    }

    private void UI_OnStochasticChanceChanged(char id, string newChanceValue, TMP_InputField stochasticChanceInputField)
    {
        float newChance;

        try
        {
            newChance = Mathf.Clamp(Convert.ToSingle(newChanceValue), 0f, 100f);
        }
        catch (FormatException)
        {
            newChance = 0f;
        }

        if (newChanceValue != string.Empty) stochasticChanceInputField.text = newChance.ToString();
        _model.Symbols[id].Rule.SetStochasticChance(newChance / 100f);
    }

    public void UI_OnAxiomChanged(string newAxiom)
    {
        _model.Axiom = newAxiom;
    }

    #endregion
}
