using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoERadiusViewer : MonoBehaviour
{
    [SerializeField] private Transform _maxRadiusIndicator;
    [SerializeField] private Transform _currentRadiusIndicator;
    
    private float _aoeRadius;
    private float _aoeDelay;
    private float _delayElapsed;
    [SerializeField] private AnimationCurve _radiusExpansionCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);


    /// <summary>
    /// Initialise the AoERadiusViewer to always show the Max Size of the AoE.
    /// </summary>
    /// <param name="radius"> The radius of the AoE Viewer.</param>
    public void Init(float radius)
    {
        // Set the scale of the maxRadiusIndicator & currentRadiusIndicator.
        float maxScale = radius * 2f;
        _maxRadiusIndicator.localScale = new Vector3(maxScale, maxScale, maxScale);
        _currentRadiusIndicator.localScale = new Vector3(maxScale, maxScale, maxScale);

        // Ensure that scale never changes.
        _aoeRadius = -1f;
    }
    /// <summary>
    /// Initialise the AoERadiusViewer to show a explosion radius that lerps to its max value over time.
    /// </summary>
    /// <param name="radius"> The target radius of the AoE Viewer.</param>
    /// <param name="time"> The time taken to reach the target radius.</param>
    public void Init(float radius, float time)
    {
        // Set values.
        this._aoeRadius = radius;
        this._aoeDelay = time;

        // Reset the elapsed time.
        _delayElapsed = 0f;


        // Set the scale of the maxRadiusIndicator.
        float maxScale = _aoeRadius * 2f;
        _maxRadiusIndicator.localScale = new Vector3(maxScale, maxScale, maxScale);
    }


    private void Update()
    {
        // Calculate the target scale of the Current Radius Indicator.
        float targetScale = Mathf.Lerp(
            a: 0f,
            b: _aoeRadius,
            t: _radiusExpansionCurve.Evaluate(_delayElapsed / _aoeDelay)
            ) * 2f;


        // If the target scale is less or equal to than 0, disable the indicator rather than setting the scale to prevent errors.
        if (targetScale <= 0)
            _currentRadiusIndicator.gameObject.SetActive(false);
        // If the target scale is greater than 0, enable the indicator and update its scale.
        else
        {
            _currentRadiusIndicator.gameObject.SetActive(true);
            _currentRadiusIndicator.localScale = new Vector3(targetScale, targetScale, targetScale);
        }


        // Increment the elapsed delay.
        _delayElapsed += Time.deltaTime;
    }
}
