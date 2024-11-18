using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    private void CreateLineGroupUIElement(char dataSymbolId, DataLine dataSymbol)
    {
        RectTransform lineGroupUIElement = Instantiate(_lineGroupPrefab, _lineGroupParent);

        TMP_Text idTextLabel = lineGroupUIElement.GetComponentInChildren<TMP_Text>();
        TMP_InputField lengthInputField = lineGroupUIElement.GetComponentInChildren<TMP_InputField>();
        // TMP_InputField colorInputField = lineGroupUIElement.GetComponentInChildren<TMP_InputField>();
        // TMP_InputField leafPrefabInputField = lineGroupUIElement.GetComponentInChildren<TMP_InputField>();

        idTextLabel.text = dataSymbolId.ToString();
        lengthInputField.text = dataSymbol.Length.ToString();
        // colorInputField.text = dataSymbol.Color.ToString();
        // leafPrefabInputField.text = dataSymbol.LeafPrefab.ToString();

        lengthInputField.onValueChanged.AddListener((string newLengthValue) => { UI_OnLengthChanged(lineGroupUIElement, newLengthValue); });
        // colorInputField.onValueChanged.AddListener((string newColorValue) => { UI_OnColorChanged(lineGroupUIElement, newColorValue); });
        // leafPrefabInputField.onValueChanged.AddListener((string newLeafPrefabValue) => { UI_OnLeafPrefabChanged(lineGroupUIElement, newLeafPrefabValue); });
    }

    #region UI Callbacks

    private void UI_OnLengthChanged(RectTransform lineGroupUIElement, string newLengthValue)
    {

    }

    #endregion
}
