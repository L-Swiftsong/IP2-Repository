using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TextElement : MonoBehaviour
{
    private TMP_Text _text;
    [SerializeField] private TMP_FontAsset _overrideDefaultFontAsset;

    [Space(5)]
    [SerializeField] private bool _useDifferentSizings = false;
    [SerializeField] private float _defaultFontSize;
    [SerializeField] private float _simplifiedFontSize;

    [Space(5)]
    [SerializeField] private bool _useDifferentColours = false;
    [SerializeField] private Color _defaultFontColour;
    [SerializeField] private Color _simplifiedFontColour;


    private void Awake() => _text = GetComponent<TMP_Text>();

    private void Start()
    {
        if (AccessibilityManager.Instance != null)
            SetFontType();

        AccessibilityManager.OnUseSimplifiedFontChanged += SetFontType;
    }

    private void OnDestroy() => AccessibilityManager.OnUseSimplifiedFontChanged -= SetFontType;


    private void SetFontType()
    {
        bool isUsingSimplified = AccessibilityManager.Instance.UseSimplifiedFont;

        // Update the font type.
        if (!isUsingSimplified && _overrideDefaultFontAsset != null)
            _text.font = _overrideDefaultFontAsset;
        else    
            _text.font = AccessibilityManager.Instance.GetFont();

        // Update the text size.
        if (_useDifferentSizings)
            _text.fontSize = isUsingSimplified ? _simplifiedFontSize : _defaultFontSize;

        // Update the text colour.
        if (_useDifferentColours)
            _text.color = isUsingSimplified ? _simplifiedFontColour : _defaultFontColour;
    }
}