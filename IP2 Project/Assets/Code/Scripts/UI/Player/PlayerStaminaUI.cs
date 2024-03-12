using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStaminaUI : MonoBehaviour
{
    [SerializeField] private ProgressBar _staminaBar;

    [SerializeField] private Transform _segmentParent;
    [SerializeField] private GameObject _dashSegmentPrefab;
    private List<GameObject> _dashSegments = new List<GameObject>();


    private void OnEnable()
    {
        PlayerMovement.OnStaminaChanged += UpdateStaminaBar;
        PlayerMovement.OnStaminaValuesChanged += StaminaValuesChanged;
    }
    private void OnDisable()
    {
        PlayerMovement.OnStaminaChanged -= UpdateStaminaBar;
        PlayerMovement.OnStaminaValuesChanged -= StaminaValuesChanged;
    }


    private void StaminaValuesChanged(float newMax, float newCost)
    {
        int newDashCount = Mathf.RoundToInt(newMax / newCost);
        SetupDashUI(newDashCount);
    }
    private void SetupDashUI(int segmentCount)
    {
        // Remove existing Dash Segemnt UI pieces.
        foreach (GameObject segment in _dashSegments)
            Destroy(segment);
        
        // Add all the dash segments to the bar.
        for (int i = 0; i < segmentCount; i++)
        {
            _dashSegments.Add(Instantiate<GameObject>(_dashSegmentPrefab, _segmentParent));
        }
    }
    private void UpdateStaminaBar(float percentage) => _staminaBar.SetValues(percentage, 1f, 0f);
}
