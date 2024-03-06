using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScaler : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Vector3 _defaultScale;
    
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _defaultScale = _rectTransform.localScale;

        AccessibilityManager.OnFontSizeChanged += UIScaleChanged;
    }


    private void UIScaleChanged() => _rectTransform.localScale = _defaultScale * AccessibilityManager.Instance.FontSizePercentage;
}
