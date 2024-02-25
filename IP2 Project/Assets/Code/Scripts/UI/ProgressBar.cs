using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ProgressBar : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("GameObject/UI/Linear Progress Bar")]
    public static void AddLinearProgressBar()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("UI/Linear Progress Bar"));
        obj.transform.SetParent(Selection.activeGameObject.transform, false);
    }
    [MenuItem("GameObject/UI/Radial Progress Bar")]
    public static void AddRadialProgressBar()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("UI/Radial Progress Bar"));
        obj.transform.SetParent(Selection.activeGameObject.transform, false);
    }
#endif


    [SerializeField] private Image _mask;
    [SerializeField] private Image _fill;
    [SerializeField] private Gradient _colourGradient;

    private float _max = 1f;
    private float _current = 0f;
    private float _min = 0f;



    private void Start()
    {
        // Ensure that values are initialised by default.
        _current = _max;
        UpdateFill();
    }

    public void SetValues(float current) => SetValues(current, _max, _min);
    public void SetValues(float current, float max) => SetValues(current, max, _min);
    public void SetValues(float current, float max, float min)
    {
        // Update cached values.
        _max = max;
        _min = Mathf.Min(min, max); // Ensure that _min is not greater than _max.
        _current = Mathf.Clamp(current, _min, _max); // Ensure that _current is between _min and _max

        // Update the progress bar's fill.
        UpdateFill();
    }


    private void UpdateFill()
    {
        // Calculate values.
        float currentOffset = _current - _min;
        float maxOffset = _max - _min;
        float fillAmount = currentOffset / maxOffset;

        // Set the mask's value.
        _mask.fillAmount = fillAmount;
        _fill.color = _colourGradient.Evaluate(fillAmount);
    }
}