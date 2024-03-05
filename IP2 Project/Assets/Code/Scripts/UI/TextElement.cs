using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TextElement : MonoBehaviour
{
    private TMP_Text _text;
    [SerializeField] private bool _isHeader;


    private void Awake() => _text = GetComponent<TMP_Text>();
    private void Start()
    {
        if (AccessibilityManager.Instance != null)
            SetFontType();

        AccessibilityManager.OnUseSimplifiedFontChanged += SetFontType;
    }

    private void OnDestroy() => AccessibilityManager.OnUseSimplifiedFontChanged -= SetFontType;


    private void SetFontType() => _text.font = _isHeader ? AccessibilityManager.Instance.GetHeaderFont() : AccessibilityManager.Instance.GetFont();
}