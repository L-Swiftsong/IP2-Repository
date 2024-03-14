using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationSegmenter : MonoBehaviour
{
    [SerializeField] private Transform _rotationPivot;
    [SerializeField] private int _angleCount;
    [SerializeField] private float _angleAddition = 90f;

    [Space(5)]
    [SerializeField] private Transform _rotationIndicator;


    private void Update()
    {
        float roundingValue = _angleCount > 0f ? 360f / _angleCount : 1;
        float zRadians = (Mathf.Round((_rotationPivot.eulerAngles.z + _angleAddition) / roundingValue) * roundingValue) * Mathf.Deg2Rad;
        Vector2 targetUp = new Vector2(Mathf.Cos(zRadians), Mathf.Sin(zRadians));
        _rotationIndicator.rotation = Quaternion.LookRotation(Vector3.forward, targetUp);
    }
}
