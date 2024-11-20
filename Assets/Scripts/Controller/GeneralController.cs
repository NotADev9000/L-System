using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GeneralController : MonoBehaviour
{
    [Header("Iteration Settings")]
    [SerializeField] private int _minIterationsAllowed = 1;
    [SerializeField] private int _maxIterationsAllowed = 10;

    [Header("UI References")]
    [SerializeField] private TMP_InputField _iterationsInputField;
    [SerializeField] private TMP_InputField _angleInputField;
    [SerializeField] private TMP_InputField _angleOffsetInputField;

    private MasterModel _model;

    public void SetModel(MasterModel model)
    {
        _model = model;

        _iterationsInputField.SetTextWithoutNotify(_model.Iterations.ToString());
        _angleInputField.SetTextWithoutNotify(_model.Angle.ToString());
        _angleOffsetInputField.SetTextWithoutNotify(_model.AngleOffset.ToString());
    }

    #region UI Callbacks

    public void UI_OnIterationsChanged(string iterations)
    {
        int iterationsNum = 1;
        try
        {
            iterationsNum = Mathf.Clamp(Convert.ToInt16(iterations), _minIterationsAllowed, _maxIterationsAllowed);
        }
        catch (FormatException) { }

        if (iterations != string.Empty) _iterationsInputField.text = iterationsNum.ToString();
        _model.Iterations = iterationsNum;
    }

    public void UI_OnAngleChanged(string angle)
    {
        float angleNum = 0f;
        try
        {
            angleNum = Convert.ToSingle(angle);
        }
        catch (FormatException) { }

        if (angle != string.Empty) _angleInputField.text = angleNum.ToString();
        _model.Angle = angleNum;
    }

    public void UI_OnAngleOffsetChanged(string angleOffset)
    {
        float angleOffsetNum = 0f;
        try
        {
            angleOffsetNum = Convert.ToSingle(angleOffset);
        }
        catch (FormatException) { }

        if (angleOffset != string.Empty) _angleOffsetInputField.text = angleOffsetNum.ToString();
        _model.AngleOffset = angleOffsetNum;
    }

    #endregion
}
