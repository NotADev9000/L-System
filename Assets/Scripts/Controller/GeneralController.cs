using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GeneralController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField _iterationsInputField;
    [SerializeField] private TMP_InputField _angleInputField;
    [SerializeField] private TMP_InputField _angleOffsetInputField;

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

        _iterationsInputField.SetTextWithoutNotify(_model.Iterations.ToString());
        _angleInputField.SetTextWithoutNotify(_model.Angle.ToString());
        _angleOffsetInputField.SetTextWithoutNotify(_model.AngleOffset.ToString());
    }

    #region Model Events

    private void SubscribeToModelEvents()
    {
        MasterModel.OnIterationsUpdated += OnIterationsUpdated;
    }

    private void UnsubscribeFromModelEvents()
    {
        MasterModel.OnIterationsUpdated -= OnIterationsUpdated;
    }

    private void OnIterationsUpdated(int iterations)
    {
        _iterationsInputField.SetTextWithoutNotify(iterations.ToString());
    }

    #endregion

    #region UI Callbacks

    public void UI_OnIterationsChanged(string iterations)
    {
        int iterationsNum = 1;
        try
        {
            iterationsNum = Convert.ToInt16(iterations);
        }
        catch (FormatException) { }

        _model.UpdateIterations(iterationsNum, iterations != string.Empty);
    }

    public void UI_OnAngleChanged(string angle)
    {
        float angleNum = 0f;
        try
        {
            angleNum = Convert.ToSingle(angle);
        }
        catch (FormatException)
        {
            if (angle != string.Empty && !angle.Contains('.')) _angleInputField.text = angleNum.ToString();
        }

        _model.Angle = angleNum;
    }

    public void UI_OnAngleOffsetChanged(string angleOffset)
    {
        float angleOffsetNum = 0f;
        try
        {
            angleOffsetNum = Convert.ToSingle(angleOffset);
        }
        catch (FormatException)
        {
            if (angleOffset != string.Empty && !angleOffset.Contains('.')) _angleOffsetInputField.text = angleOffsetNum.ToString();
        }

        _model.AngleOffset = angleOffsetNum;
    }

    #endregion
}
