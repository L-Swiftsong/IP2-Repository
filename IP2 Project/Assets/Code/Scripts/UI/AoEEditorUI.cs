using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AoEEditorUI : MonoBehaviour
{
    [SerializeField] private FlexibleColorPicker _flexibleColorPicker;

    [Space(10)]
    public UnityEvent<Color> OnColourChangeSaved;
    public UnityEvent OnColourChangeCancelled;


    public void SetColour(Color initialColour) => _flexibleColorPicker.SetColor(initialColour);

    public void SaveValues() => OnColourChangeSaved?.Invoke(_flexibleColorPicker.GetColor());
    public void Cancel() => OnColourChangeCancelled?.Invoke();
}
