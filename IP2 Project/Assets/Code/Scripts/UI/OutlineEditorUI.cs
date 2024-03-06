using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class OutlineEditorUI : MonoBehaviour
{
    [SerializeField] private FlexibleColorPicker _flexibleColorPicker;
    [SerializeField] private TMP_Text _thicknessText;
    private float _currentThickness = 1.0f;


    [Space(10)]
    public UnityEvent<Color, float> OnOutlineAccepted;
    public UnityEvent OnOutlineCancelled;


    public void SetColour(Color initialColour) => _flexibleColorPicker.SetColor(initialColour);
    public void SetThickness(float newVal)
    {
        _currentThickness = Mathf.RoundToInt(newVal * 10f) / 10f;
        _thicknessText.text = _currentThickness.ToString("#.0");
    }


    public void SaveValues() => OnOutlineAccepted?.Invoke(_flexibleColorPicker.GetColor(), _currentThickness);
    public void Cancel() => OnOutlineCancelled?.Invoke();
}
