using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerComboUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _comboText;


    private void Start() => ComboValueChanged(0f);
    public void OnEnable() => PlayerCombo.OnComboChanged += ComboValueChanged;
    public void OnDisable() => PlayerCombo.OnComboChanged -= ComboValueChanged;


    private void ComboValueChanged(float newValue) => _comboText.text = string.Format("x{0}", newValue.ToString("#.0"));
}
