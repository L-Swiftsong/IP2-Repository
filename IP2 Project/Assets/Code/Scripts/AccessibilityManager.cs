using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AccessibilityManager : MonoBehaviour
{
    public static AccessibilityManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }


    [Header("Fonts")]
    [SerializeField] private bool _useSimplifiedFont;
    public bool UseSimplifiedFont
    {
        get => _useSimplifiedFont;
        set
        {
            _useSimplifiedFont = value;
            OnUseSimplifiedFontChanged?.Invoke();
        }
    }
    public static Action OnUseSimplifiedFontChanged;


    [Space(10)]
    [SerializeField] private TMP_FontAsset StandardFont;
    [SerializeField] private TMP_FontAsset SimplifiedFont;
    public TMP_FontAsset GetFont() => UseSimplifiedFont ? SimplifiedFont : StandardFont;


    [Space(10)]
    [SerializeField] private TMP_FontAsset HeaderFont;
    [SerializeField] private TMP_FontAsset SimplifiedHeaderFont;
    public TMP_FontAsset GetHeaderFont() => UseSimplifiedFont ? SimplifiedHeaderFont : HeaderFont;


    // Font Size
    private float _fontSizePercentage = 1f;
    public float FontSizePercentage
    {
        get => _fontSizePercentage;
        set
        {
            _fontSizePercentage = value;
            OnFontSizeChanged?.Invoke();
        }
    }
    public static Action OnFontSizeChanged;
}
